using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using HDF5.Api.Disposables;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.NativeMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5.Api.H5Attributes;

public class H5StringAttribute : H5Attribute<string, H5StringAttribute, H5StringType>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
    }

    public override H5StringType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5StringType(h));
    }

    public override string Read()
    {
        return Read(this);
    }

    public override void Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);

        WriteImpl(this, value);
    }

    public override void Write([DisallowNull] IEnumerable<string> value)
    {
        Guard.IsNotNull(value);

        WriteImpl(this, value);
    }

    public static H5StringAttribute Create(long handle)
    {
        return new H5StringAttribute(handle);
    }

    private static unsafe void WriteImpl(H5StringAttribute attribute, string value)
    {
        WriteImpl(attribute, Enumerable.Repeat(value, 1));
        return;
    }

    private static unsafe void WriteImpl(H5StringAttribute attribute, IEnumerable<string> values)
    {
        using var type = attribute.GetAttributeType();
        using var space = attribute.GetSpace();

        var cls = type.Class;
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        var characterSet = type.CharacterSet;

        var bytes = characterSet switch
        {
            CharacterSet.Ascii => ToAscii(values),
            CharacterSet.Utf8 => ToUtf8(values),
            _ => throw new InvalidEnumArgumentException($"Unknown CharacterSet:{characterSet}.")
        };

        if (type.IsVariableLength)
        {
#if NET7_0_OR_GREATER
            H5A.write(attribute, type, bytes).ThrowIfError();
#else
            var pinned = bytes.Select(b => new PinnedObject(b));

            try
            {
                using var pinnedArray = new PinnedObject(pinned.Select(p => (IntPtr)p).ToArray());

                H5A.write(attribute, type, new IntPtr(pinnedArray)).ThrowIfError();
            }
            finally
            {
                foreach (var p in pinned) { p.Dispose(); }
            }
#endif
        }
        else
        {
            int storageSize = attribute.StorageSize; // = type.Size * count

            // TODO: optionally truncate to nearest character (not byte) to fit fixed length buffer
            var tooLong = (bytes.FirstOrDefault(b => b.Length > type.Size));

            if (tooLong != null)
            {
                throw new ArgumentOutOfRangeException(
                    $"At least one string requires {tooLong.Length} bytes of storage which is greater than the allocated fixed storage size of {type.Size} bytes.");
            }

            using var spanOwner = SpanOwner<byte>.Allocate(storageSize);

            for (int i = 0; i < bytes.Length; i++)
            {
                var span = new Span<byte>(bytes[i]);
                span.CopyTo(spanOwner.Span.Slice(i * type.Size, type.Size));
            }

            H5AAdapter.Write(attribute, type, spanOwner.Span);
        }

        // TODO: optionally throw if writing a string containing non-ASCII characters when characterSet = Ascii
        static unsafe byte[][] ToAscii(IEnumerable<string> values)
        {
            return values.Select(v => Encoding.ASCII.GetBytes(v + '\0')).ToArray();
        }

        static unsafe byte[][] ToUtf8(IEnumerable<string> values)
        {
            return values.Select(v => Encoding.UTF8.GetBytes(v + '\0')).ToArray();
        }
    }

    private static unsafe string Read(H5StringAttribute attribute)
    {
        using var type = attribute.GetAttributeType();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO: optionally convert UTF-8 to Ascii with <?> markers
        // TODO: generalise to NPoints >= 0

        if (count != 1 ||
            dims.Any(d =>
                d.UpperLimit >
                1)) // NOTE: dims.Count could be > 0 with count == 1 where we have an array of [1]..[1] with one element
        {
            throw new H5Exception("Attribute is not scalar.");
        }

        var cls = type.Class;
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        if (type.IsVariableLength)
        {
            if (count < 256 / sizeof(nint))
            {
                Span<nint> buffer = stackalloc nint[(int)count];
                return ReadVariableStrings(buffer);
            }

            return ReadVariableStrings(new Span<nint>(new nint[count]));

            string ReadVariableStrings(Span<nint> buffer)
            {
                fixed (nint* bufferPtr = buffer)
                {
                    // IntPtr is a struct so no need to pin
                    var ptr = new IntPtr(bufferPtr);
                    try
                    {
                        // TODO: move to H5AAdapter
                        H5A.read(attribute, type, ptr).ThrowIfError();

                        if (buffer[0] == 0)
                        {
                            // If the attribute was never written (or do we allow nulls?)
                            return string.Empty;
                        }
                        else
                        {
                            // NOTE: no way to retrieve size of variable length buffer.
                            // Only search for null up to a fixed length.
                            Span<byte> bytes = new((byte*)buffer[0], H5Global.MaxVariableLengthStringBuffer);
                            var nullTerminatorIndex = System.MemoryExtensions.IndexOf(bytes, (byte)0);
                            if (nullTerminatorIndex != -1)
                            {
                                return Encoding.UTF8.GetString((byte*)buffer[0], nullTerminatorIndex);
                            }
                            else
                            {
                                throw new H5Exception(
                                    $"Unable to locate end of string within first {H5Global.MaxVariableLengthStringBuffer} bytes." +
                                    " If required increase the value in {nameof(H5Global)}.{nameof(H5Global.MaxVariableLengthStringBuffer)}).");
                            }
                        }
                    }
                    finally
                    {
                        // TODO: check this really works
                        H5DAdapter.ReclaimVariableLengthMemory(type, space, (byte**)bufferPtr);
                    }
                }
            }
        }
        // ReSharper disable once RedundantIfElseBlock
        else
        {
            int storageSize = attribute.StorageSize;

#if NET7_0_OR_GREATER
            if (storageSize < 256)
            {
                Span<byte> buffer = stackalloc byte[storageSize + 1];
                return ReadString(buffer);
            }
            else
            {
                using var spanOwner = SpanOwner<byte>.Allocate(storageSize + 1);
                return ReadString(spanOwner.Span);
            }

            string ReadString(Span<byte> buffer)
            {
                // TODO: move to H5AAdapter
                H5A.read(attribute, type, buffer).ThrowIfError();

                var nullTerminatorIndex = System.MemoryExtensions.IndexOf(buffer, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
            }
#else
            var buffer = new byte[storageSize + 1];
            fixed (byte* bufferPtr = buffer)
            {
                H5A.read(attribute, type, bufferPtr).ThrowIfError();

                Span<byte> bytes = buffer;
                var nullTerminatorIndex = System.MemoryExtensions.IndexOf(bytes, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
            }
#endif
        }
    }
}

