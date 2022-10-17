#if NETSTANDARD
using HDF5.Api.Disposables;
using HDF5.Api.Utils;
#endif
using CommunityToolkit.Diagnostics;
#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using static HDF5.Api.NativeMethods.H5A;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal unsafe static class H5AAdapter
{
    internal static void Close(H5Attribute attribute)
    {
        int err = close(attribute);

        err.ThrowIfError();
    }

    internal static H5Attribute Create<T>(
        H5Object<T> h5Object,
        string name,
        H5Type type,
        H5Space space) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        // TODO: cache/singleton default CreationPropertyList
        using var creationPropertyList = CreateCreationPropertyList(CharacterSet.Utf8);

        // ensure CharacterEncoding == CharacterSet.Utf8
        creationPropertyList.CharacterEncoding = CharacterSet.Utf8;

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
            var name = info.cset switch
            {
#if NET7_0_OR_GREATER
                H5T.cset_t.ASCII or H5T.cset_t.UTF8 => Marshal.PtrToStringUTF8(intPtrName),
#else
                H5T.cset_t.ASCII or H5T.cset_t.UTF8 => MarshalExtensions.PtrToStringUTF8(intPtrName),
#endif
                _ => throw new InvalidEnumArgumentException($"Unexpected character set {info.cset} when enumerating attribute names."),
            };

            Guard.IsNotNull(name);

            names.Add(name);

            return 0;
        }
    }

    // TODO: use or remove
    /*    internal static info_t GetInfoByName<T>(H5Object<T> h5Object,
            string objectName, string attributeName, H5PropertyList? linkAccessPropertyList = null)
            where T : H5Object<T>
        {
            info_t info = default;
            int err = 0;

    #if NET7_0_OR_GREATER
            err = get_info_by_name(h5Object, objectName, attributeName, ref info, linkAccessPropertyList);
    #else
            fixed (byte* objectNamePtr = Encoding.UTF8.GetBytes(objectName))
            fixed (byte* attributeNamePtr = Encoding.UTF8.GetBytes(attributeName))
            {
                err = get_info_by_name(h5Object, objectNamePtr, attributeNamePtr, ref info, linkAccessPropertyList);
            }
    #endif

            err.ThrowIfError();
            return info;
        }
    */

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
        return H5PAdapter.Create(H5P.ATTRIBUTE_CREATE, h =>
        {
            return new H5AttributeCreationPropertyList(h)
            {
                CharacterEncoding = encoding
            };
        });
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

        // TODO: handle dims.Count > 0 where NPoints=1
        // TODO: generalise to NPoints >= 0
        // TODO: handle fixed/variable length string

        if (count != 1 || dims.Count != 0)
        {
            throw new Hdf5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();
        if (cls != H5Class.String)
        {
            throw new Hdf5Exception($"Attribute is of class '{cls}' when expecting '{H5Class.String}'.");
        }


        int storageSize = attribute.StorageSize;
        var characterSet = type.GetCharacterSet();
        bool isVariableLength = type.IsVariableLengthString();

        if (isVariableLength)
        {
            return "<TODO>";
        }
        else
        {
#if NET7_0_OR_GREATER
            // TODO: could optimise with stackalloc for small strings
            using var spanOwner = SpanOwner<byte>.Allocate(storageSize);
            var buffer = spanOwner.Span;
            int err = read(attribute, type, buffer);
            err.ThrowIfError();

            var nullTerminatorIndex = MemoryExtensions.IndexOf(spanOwner.Span, (byte)0);
            nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
            return Encoding.UTF8.GetString(buffer[0..nullTerminatorIndex]);
#else
            using var buffer = new GlobalMemory(storageSize + 1);
            int err = read(attribute, type, buffer.IntPtr);
            err.ThrowIfError();

            Span<byte> bytes = new Span<byte>(buffer.IntPtr.ToPointer(), storageSize + 1);
            var nullTerminatorIndex = MemoryExtensions.IndexOf(bytes, (byte)0);
            nullTerminatorIndex = nullTerminatorIndex < 0 ? storageSize : nullTerminatorIndex;
            return Encoding.UTF8.GetString((byte*)buffer.IntPtr.ToPointer(), nullTerminatorIndex);
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

        int size = attribute.StorageSize;

        if (size != Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
        }

#if NET7_0_OR_GREATER
        if (size < 256)
        {
            Span<T> buf = stackalloc T[1];
            read(attribute, type, MemoryMarshal.AsBytes(buf));
            return buf[0];
        }
        else
        {
            // TODO: is this required - most unmanaged types are small?
            using var buf = SpanOwner<T>.Allocate(1);
            read(attribute, type, MemoryMarshal.AsBytes(buf.Span));
            return buf.Span[0];
        }
#else
        T result = default;
        int err = read(attribute, type, new IntPtr(&result));
        err.ThrowIfError();
        return result;
#endif
    }

#if NET7_0_OR_GREATER
    internal static void Write<T>(H5Attribute attribute, T value) where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();

        using var type = attribute.GetH5Type();

        if (size < 256)
        {
            Span<T> buffer = stackalloc T[1] { value };
            Write(attribute, type, MemoryMarshal.Cast<T, byte>(buffer));
        }
        else
        {
            using var buffer = SpanOwner<T>.Allocate(size);
            buffer.Span[0] = value;
            Write(attribute, type, MemoryMarshal.Cast<T, byte>(buffer.Span));
        }
    }

    internal static void Write(H5Attribute attribute, H5Type type, Span<byte> buffer)
    {
        int err = write(attribute, type, buffer);

        err.ThrowIfError();
    }
#endif

#if NETSTANDARD
    internal static void Write<T>(H5Attribute attribute, T value) where T : unmanaged
    {
        unsafe
        {
            void* p = &value;
            {
                using var type = attribute.GetH5Type();

                Write(attribute, type, new IntPtr(p));
            }
        }
    }

    internal static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = write(attribute, type, buffer);

        err.ThrowIfError();
    }
#endif

    internal static void WriteString(H5Attribute attribute, string value)
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();
        var dims = space.GetSimpleExtentDims();

        if (count != 1 || dims.Count != 0)
        {
            throw new Hdf5Exception("Attribute is not scalar.");
        }

        var cls = type.GetClass();
        if (cls != H5Class.String)
        {
            throw new Hdf5Exception($"Attribute is of class '{cls}' when expecting '{H5Class.String}'.");
        }

        var characterSet = type.GetCharacterSet();
        bool isVariableLength = type.IsVariableLengthString();

        var bytes = characterSet switch
        {
            // we absolutely need to add '\0' :)
            CharacterSet.Ascii => Encoding.ASCII.GetBytes(value + '\0'),
            CharacterSet.Utf8 => Encoding.UTF8.GetBytes(value + '\0'),
            _ => throw new InvalidEnumArgumentException($"Unknown CharacterSet:{characterSet}."),
        };

        if (isVariableLength)
        {
#if NETSTANDARD
            unsafe
            {
                fixed (void* fixedBytes = bytes)
                {
                    var stringArray = new IntPtr[1] { new IntPtr(fixedBytes) };

                    fixed (void* stringArrayPtr = stringArray)
                    {
                        Write(attribute, type, new IntPtr(stringArrayPtr));
                    }
                }
            }
#endif

#if NET7_0_OR_GREATER
            // TODO: indirection
            //Write(attribute, type, bytes.AsSpan());
#endif
        }
        else
        {
            int storageSize = attribute.StorageSize;

            if (bytes.Length > storageSize)
            {
                throw new ArgumentOutOfRangeException($"The string requires {bytes.Length} storage which than the allocated fixed storage size of {storageSize} bytes.");
            }

#if NETSTANDARD
            unsafe
            {
                fixed (void* fixedBytes = bytes)
                {
                    Write(attribute, type, new IntPtr(fixedBytes));
                }
            }
#endif

#if NET7_0_OR_GREATER
            Write(attribute, type, bytes.AsSpan());
#endif
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

        type.SetCharacterSet(cset);
        type.SetPadding(padding);

        using var memorySpace = H5SAdapter.CreateScalar();
        return Create(h5Object, name, type, memorySpace);
    }
}

