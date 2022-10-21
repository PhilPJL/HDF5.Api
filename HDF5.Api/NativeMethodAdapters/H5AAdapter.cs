#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.Utils;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using static HDF5.Api.NativeMethods.H5A;
using System.Linq;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal static unsafe class H5AAdapter
{
    internal static void Close(H5Attribute attribute)
    {
        int err = close(attribute);

        err.ThrowIfError();
    }

    internal static H5Attribute Create<T, TA>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        throw new NotImplementedException();
    }

    internal static H5Attribute Create<T>(
        H5Object<T> h5Object,
        string name,
        H5Type type,
        H5Space space) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var creationPropertyList = CreateCreationPropertyList(CharacterSet.Utf8);

        long h;

#if NET7_0_OR_GREATER
        h = create(h5Object, name, type, space, creationPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = create(h5Object, nameBytesPtr, type, space, creationPropertyList);
        }
#endif

        h.ThrowIfInvalidHandleValue();
        return new H5Attribute(h);
    }

    internal static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int err = 0;

#if NET7_0_OR_GREATER
        err = delete(h5Object, name);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = delete(h5Object, nameBytesPtr);
        }
#endif

        err.ThrowIfError();
    }

    internal static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int err = 0;

#if NET7_0_OR_GREATER
        err = exists(h5Object, name);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = exists(h5Object, nameBytesPtr);
        }
#endif

        err.ThrowIfError();
        return err > 0;
    }

    internal static IEnumerable<string> GetAttributeNames<T>(H5Object<T> h5Object) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        ulong idx = 0;

        var names = new List<string>();

        int err = iterate(h5Object,
            H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

        err.ThrowIfError();

        return names;

        int Callback(long id, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            try
            {
                var name = info.cset switch
                {
#if NET7_0_OR_GREATER
                    H5T.cset_t.ASCII or H5T.cset_t.UTF8 => Marshal.PtrToStringUTF8(intPtrName),
#else
                    H5T.cset_t.ASCII or H5T.cset_t.UTF8 => MarshalHelpers.PtrToStringUTF8(intPtrName),
#endif
                    // Don't throw inside callback - see HDF docs
                    _ => string.Empty
                };

                if (name != null)
                {
                    names.Add(name);
                }

                return 0;
            }
            catch
            {
                // Don't throw inside callback - see HDF docs
                return -1;
            }
        }
    }

    /// <summary>
    /// Get copy of property list used when creating the attribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    internal static H5AttributeCreationPropertyList GetCreationPropertyList(H5Attribute attribute)
    {
        return H5PAdapter.GetPropertyList(attribute, get_create_plist, h => new H5AttributeCreationPropertyList(h));
    }

    /// <summary>
    /// Create a new attribute creation property list
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding)
    {
        // TODO: ideally cache two CreationPropertyLists (Utf8/Ascii) and exclude them from handle tracking.
        return H5PAdapter.Create(H5P.ATTRIBUTE_CREATE, h =>
        {
            return new H5AttributeCreationPropertyList(h)
            {
                CharacterEncoding = encoding
            };
        });
    }

    internal static string GetName(H5Attribute attribute)
    {
#if NET7_0_OR_GREATER
        return MarshalHelpers.GetName(attribute, (long attr_id, Span<byte> name, nint size) => get_name(attr_id, size, name));
#else
        return MarshalHelpers.GetName(attribute, (long attr_id, byte* name, nint size) => get_name(attr_id, size, name));
#endif
    }

    internal static H5Space GetSpace(H5Attribute attribute)
    {
        var space = get_space(attribute);

        space.ThrowIfError();

        return new H5Space(space);
    }

    internal static int GetStorageSize(H5Attribute attribute)
    {
        return (int)get_storage_size(attribute);
    }

    internal static H5Type GetType(H5Attribute attribute)
    {
        long typeHandle = get_type(attribute);
        typeHandle.ThrowIfInvalidHandleValue();
        return new H5Type(typeHandle);
    }

    internal static H5Attribute Open<T>(H5Object<T> h5Object, string name)
        where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        long h = 0;

#if NET7_0_OR_GREATER
        h = open(h5Object, name);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(h5Object, nameBytesPtr);
        }
#endif

        h.ThrowIfInvalidHandleValue();

        return new H5Attribute(h);
    }

    internal static DateTime ReadDateTime(H5Attribute attribute)
    {
        return DateTime.FromOADate(Read<double>(attribute));
    }

    internal static string ReadString(H5Attribute attribute)
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO: generalise to NPoints >= 0

        if (count != 1 || dims.Any(d => d.UpperLimit > 1)) // NOTE: dims.Count could be > 0 with count == 1 where we have an array of [1]..[1] with one element
        {
            throw new Hdf5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new Hdf5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        if (type.IsVariableLengthString())
        {
            if (count < 256 / sizeof(nint))
            {
                Span<nint> buffer = stackalloc nint[(int)count];
                return ReadVariableStrings(buffer);
            }
            else
            {
#if NET7_0_OR_GREATER
                using var spanOwner = SpanOwner<nint>.Allocate((int)count);
                return ReadVariableStrings(spanOwner.Span);
#else
                return ReadVariableStrings(new Span<nint>(new nint[count]));
#endif
            }

            string ReadVariableStrings(Span<nint> buffer)
            {
                fixed (nint* bufferPtr = buffer)
                {
                    // IntPtr is a struct so no need to pin
                    var ptr = new IntPtr(bufferPtr);
                    try
                    {
                        int err = read(attribute, type, ptr);
                        err.ThrowIfError();

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
                            var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
                            if (nullTerminatorIndex != -1)
                            {
                                return Encoding.UTF8.GetString((byte*)buffer[0], nullTerminatorIndex);
                            }
                            else
                            {
                                throw new Hdf5Exception(
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
                int err = read(attribute, type, buffer);
                err.ThrowIfError();

                var nullTerminatorIndex = MemoryExtensions.IndexOf(buffer, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
            }
#else
            var buffer = new byte[(storageSize + 1)];
            fixed (byte* bufferPtr = buffer)
            {
                int err = read(attribute, type, bufferPtr);
                err.ThrowIfError();

                Span<byte> bytes = buffer;
                var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
                nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
                return Encoding.UTF8.GetString(bufferPtr, nullTerminatorIndex);
            }
#endif
        }
    }

    internal static T Read<T>(H5Attribute attribute) where T : unmanaged
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        long count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        // TODO: handle dims.Count > 0 where NPoints=1
        // TODO: generalise to NPoints >= 0

        if (count != 1 || dims.Count != 0)
        {
            throw new Hdf5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();

        using var nativeType = H5Type.GetNativeType<T>();
        var expectedCls = H5TAdapter.GetClass(nativeType);

        if (cls != expectedCls)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
        }

        int attributeStorageSize = attribute.StorageSize;
        int marshalSize = Marshal.SizeOf<T>();

        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, marshalSize);

        T result = default;
        int err = read(attribute, type, new IntPtr(&result));
        err.ThrowIfError();
        return result;
    }

    internal static void Write<T>(H5Attribute attribute, T value) where T : unmanaged
    {
        using var type = attribute.GetH5Type();

        Write(attribute, type, value);
    }

    internal static void Write<T>(H5Attribute attribute, H5Type type, T value) where T : unmanaged
    {
        var marshalSize = Marshal.SizeOf<T>();
        int attributeStorageSize = attribute.StorageSize;
        H5ThrowHelpers.ThrowOnAttributeStorageMismatch<T>(attributeStorageSize, marshalSize);

        unsafe
        {
            // No need to pin 
            void* p = &value;
            {
                Write(attribute, type, new IntPtr(p));
            }
        }
    }

    internal static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = write(attribute, type, buffer);

        err.ThrowIfError();
    }

    internal static void WriteString(H5Attribute attribute, string value)
    {
        // TODO: handle array of strings

        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        if (count != 1 || dims.Count != 0)
        {
            throw new Hdf5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();
        if (cls != DataTypeClass.String)
        {
            throw new Hdf5Exception($"Attribute is of class '{cls}' when expecting '{DataTypeClass.String}'.");
        }

        var characterSet = type.CharacterSet;

        var bytes = characterSet switch
        {
            // we absolutely need to add '\0' :)
            CharacterSet.Ascii => Encoding.ASCII.GetBytes(value + '\0'),
            CharacterSet.Utf8 => Encoding.UTF8.GetBytes(value + '\0'),
            _ => throw new InvalidEnumArgumentException($"Unknown CharacterSet:{characterSet}."),
        };

        if (type.IsVariableLengthString())
        {
            unsafe
            {
                fixed (void* fixedBytes = bytes)
                {
                    var stringArray = new IntPtr[1] { new(fixedBytes) };

                    fixed (void* stringArrayPtr = stringArray)
                    {
                        Write(attribute, type, new IntPtr(stringArrayPtr));
                    }
                }
            }
        }
        else
        {
            int storageSize = attribute.StorageSize;

            if (bytes.Length > storageSize)
            {
                throw new ArgumentOutOfRangeException($"The string requires {bytes.Length} storage which is greater than the allocated fixed storage size of {storageSize} bytes.");
            }

            unsafe
            {
                fixed (void* fixedBytes = bytes)
                {
                    Write(attribute, type, new IntPtr(fixedBytes));
                }
            }
        }
    }

    internal static void Write(H5Attribute attribute, DateTime value)
    {
        Write(attribute, value.ToOADate());
    }

    internal static H5Attribute CreateStringAttribute<T>(
        H5Object<T> h5Object, string name, int fixedStorageLength,
        CharacterSet cset, StringPadding padding) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var type = fixedStorageLength != 0
            ? H5TAdapter.CreateFixedLengthStringType(fixedStorageLength)
            : H5TAdapter.CreateVariableLengthStringType();

        type.CharacterSet = cset;
        type.StringPadding = padding;

        using var memorySpace = H5SAdapter.CreateScalar();
        return Create(h5Object, name, type, memorySpace);
    }
}

