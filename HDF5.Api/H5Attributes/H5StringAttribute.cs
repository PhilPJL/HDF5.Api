using CommunityToolkit.Diagnostics;
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;
using HDF5.Api.NativeMethods;
using System.Linq;

namespace HDF5.Api.H5Attributes;

public class H5StringAttribute : H5Attribute<string, H5StringAttribute, H5StringType>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
    }

    public override H5StringType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5StringType(h));
    }

    public override string Read(bool verifyType = false)
    {
        return Read(this);
    }

    public override H5StringAttribute Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);

        Write(this, value);

        return this;
    }

    private static unsafe void Write(H5StringAttribute attribute, string value)
    {
        // TODO: handle array of strings

        using var type = attribute.GetAttributeType();
        using var space = attribute.GetSpace();

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        if (count != 1 || dims.Count != 0)
        {
            throw new H5Exception("Attribute is not scalar.");
        }

        var characterSet = type.CharacterSet;

        // TODO: optionally throw if writing a string containing non-ASCII characters when characterSet = Ascii
        // TODO: optionally silently truncate to nearest character (not byte)

        var bytes = characterSet switch
        {
            // we absolutely need to add '\0' :)
            CharacterSet.Ascii => Encoding.ASCII.GetBytes(value + '\0'),
            CharacterSet.Utf8 => Encoding.UTF8.GetBytes(value + '\0'),
            _ => throw new InvalidEnumArgumentException($"Unknown CharacterSet:{characterSet}.")
        };

        if (type.IsVariableLengthString())
        {
            fixed (void* fixedBytes = bytes)
            {
                var stringArray = new IntPtr[] { new(fixedBytes) };

                fixed (void* stringArrayPtr = stringArray)
                {
                    H5AAdapter.Write(attribute, type, new IntPtr(stringArrayPtr));
                }
            }
        }
        else
        {
            int storageSize = attribute.StorageSize;

            if (bytes.Length > storageSize)
            {
                throw new ArgumentOutOfRangeException(
                    $"The string requires {bytes.Length} storage which is greater than the allocated fixed storage size of {storageSize} bytes.");
            }

            fixed (void* fixedBytes = bytes)
            {
                H5AAdapter.Write(attribute, type, new IntPtr(fixedBytes));
            }
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

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new H5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        if (type.IsVariableLengthString())
        {
            if (count < 256 / sizeof(nint))
            {
                Span<nint> buffer = stackalloc nint[(int)count];
                return ReadVariableStrings(buffer);
            }

            // TODO: performance check
            //#if NET7_0_OR_GREATER
            //            using var spanOwner = SpanOwner<nint>.Allocate((int)count);
            //            return ReadVariableStrings(spanOwner.Span);
            //#else
            return ReadVariableStrings(new Span<nint>(new nint[count]));
            //#endif

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

