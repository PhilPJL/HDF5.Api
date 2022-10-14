using CommunityToolkit.Diagnostics;
#if NETSTANDARD
using HDF5.Api.Disposables;
#endif
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
internal static class H5AAdapter
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
        H5Space space,
        H5PropertyList? creationPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        var h = create(h5Object, name, type, space, creationPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5Attribute(h);
    }

    internal static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int err = delete(h5Object, name);

        err.ThrowIfError();
    }

    internal static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        int err = exists(h5Object, name);

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
            string? name = null;

            switch (info.cset)
            {
                case H5T.cset_t.ASCII:
                    name = Marshal.PtrToStringAnsi(intPtrName);
                    break;
                case H5T.cset_t.UTF8:
#if NETSTANDARD
                    // TODO: does this work for UTF8?
                    name = Marshal.PtrToStringAuto(intPtrName);
#endif
#if NET7_0_OR_GREATER
                    name = Marshal.PtrToStringUTF8(intPtrName);
#endif
                    break;
                default:
                    break;
            }

#if DEBUG
            // TODO: throw in release builds?
            Guard.IsNotNull(name);
#endif

            if (name != null)
            {
                names.Add(name.Trim('\0'));
            }

            return 0;
        }
    }

    internal static info_t GetInfoByName<T>(H5Object<T> h5Object,
        string objectName, string attributeName, H5PropertyList? linkAccessPropertyList = null)
        where T : H5Object<T>
    {
        info_t info = default;
        int err = get_info_by_name(h5Object, objectName, attributeName, ref info, linkAccessPropertyList);
        err.ThrowIfError();
        return info;
    }

    /// <summary>
    /// Get copy of property list used to create the attribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    internal static H5PropertyList GetPropertyList(H5Attribute attribute, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(attribute, get_create_plist),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
    }

    internal static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.ATTRIBUTE_CREATE),
            _ => throw new InvalidEnumArgumentException(nameof(listType), (int)listType, typeof(PropertyListType)),
        };
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

        long h = open(h5Object, name);

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

        bool isVariableLength = type.IsVariableLengthString();

        var size = GetStorageSize(attribute);

#if NET7_0_OR_GREATER
        if (size < 256)
        {
            Span<byte> buffer = stackalloc byte[size];
            read(attribute, type, buffer);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
        else
        {
            var buffer = SpanOwner<byte>.Allocate(size);
            read(attribute, type, buffer.Span);
            return Encoding.UTF8.GetString(buffer.Span).TrimEnd('\0');
        }
#else
        using var buffer = new GlobalMemory(size + 1);
        read(attribute, type, buffer.IntPtr);

        // TODO: Ascii/UTF8
        return Marshal.PtrToStringAnsi(buffer.IntPtr, size).TrimEnd('\0');
#endif
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

        int size = GetStorageSize(attribute);

        if (size != Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
        }

#if NET7_0_OR_GREATER
        if (size < 256)
        {
            Span<T> buf = stackalloc T[size];
            read(attribute, type, MemoryMarshal.AsBytes(buf));
            return buf[0];
        }
        else
        {
            using var buf = SpanOwner<T>.Allocate(size);
            read(attribute, type, MemoryMarshal.AsBytes(buf.Span));
            return buf.Span[0];
        }
#else
        unsafe
        {
            T result = default;
            int err = read(attribute, type, new IntPtr(&result));
            err.ThrowIfError();
            return result;
        }
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
        using var pinned = new PinnedObject(value);
        using var type = attribute.GetH5Type();

        Write(attribute, type, pinned);
    }

    internal static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = write(attribute, type, buffer);

        err.ThrowIfError();
    }
#endif

    // TODO: fixed/variable
    // TODO: ascii/UTF8
    // TODO: zero length string?
    internal static void Write(H5Attribute attribute, string value, int maxLength = 0)
    {
        // TODO: UTF8/Ascii
        // TODO: fixed/variable length
        // TODO: zero length string (EMPTY)

        value ??= string.Empty;

        maxLength = maxLength <= 0 ? value.Length : Math.Min(value.Length, maxLength);

        // TODO: variable length string

#pragma warning disable IDE0057 // Use range operator
        string subString = value.Length > maxLength ? value.Substring(0, maxLength) : value;
#pragma warning restore IDE0057 // Use range operator

        // TODO: confirm type matches
        using var typeId = attribute.GetH5Type();

#if NETSTANDARD
        // TODO: encoding UTF8?
        byte[] sourceBytes = H5TypeAdapterBase.Ascii.GetBytes(subString);
        using var pinned = new PinnedObject(sourceBytes);
        Write(attribute, typeId, pinned);
#endif

#if NET7_0_OR_GREATER
        // TODO: encoding UTF8?
        var span = H5TypeAdapterBase.Ascii.GetBytes(value).AsSpan();
        Write(attribute, typeId, span);
#endif
    }

    internal static void Write(H5Attribute attribute, DateTime value)
    {
        Write(attribute, value.ToOADate());
    }

    internal static H5Attribute CreateStringAttribute<T>(
        H5Object<T> h5Object, string name, int fixedLength, 
        CharacterSet cset, StringPadding padding, H5PropertyList? creationPropertyList) where T : H5Object<T>
    {
        h5Object.AssertHasWithAttributesHandleType();

        using var type = fixedLength != 0
            ? H5TAdapter.CreateFixedLengthStringType(fixedLength)
            : H5TAdapter.CreateVariableLengthStringType();

        type.SetCharacterSet(cset);
        type.SetPadding(padding);

        using var memorySpace = H5SAdapter.CreateScalar();
        return Create(h5Object, name, type, memorySpace, creationPropertyList);
    }
}

