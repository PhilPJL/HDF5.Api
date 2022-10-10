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
    public static void Close(H5Attribute attribute)
    {
        int err = close(attribute);

        err.ThrowIfError();
    }

    public static H5Attribute Create<T>(H5Object<T> h5Object, string name, H5Type type, H5Space space,
        H5PropertyList? creationPropertyList = null) 
        where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        var h = create(h5Object, name, type, space, creationPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5Attribute(h);
    }

    public static H5Attribute Open<T>(H5Object<T> h5Object, string name) 
        where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        long h = open(h5Object, name);

        h.ThrowIfInvalidHandleValue();

        return new H5Attribute(h);
    }

    public static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = delete(h5Object, name);

        err.ThrowIfError();
    }

    public static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = exists(h5Object, name);

        err.ThrowIfError();
        return err > 0;
    }

    public static IEnumerable<string> ListAttributeNames<T>(H5Object<T> h5Object) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = iterate(h5Object,
            H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

        err.ThrowIfError();

        return names;

        int Callback(long id, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);
            Guard.IsNotNull(name);

            names.Add(name);
            return 0;
        }
    }

    public static info_t GetInfoByName<T>(H5Object<T> h5Object,
        string objectName, string attributeName, H5PropertyList? linkAccessPropertyList = null) 
        where T : H5Object<T>
    {
        info_t info = default;
        int err = get_info_by_name(h5Object, objectName, attributeName, ref info, linkAccessPropertyList);
        err.ThrowIfError();
        return info;
    }

    // TODO: Span<> variant
    public static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = write(attribute, type, buffer);

        err.ThrowIfError();
    }

    public static H5Space GetSpace(H5Attribute attribute)
    {
        var space = get_space(attribute);

        space.ThrowIfError();

        return new H5Space(space);
    }

    public static int GetStorageSize(H5Attribute attribute)
    {
        return (int)get_storage_size(attribute);
    }

    public static H5Type GetType(H5Attribute attribute)
    {
        long typeHandle = get_type(attribute);
        typeHandle.ThrowIfInvalidHandleValue();
        return new H5Type(typeHandle);
    }

    public static string ReadString(H5Attribute attribute)
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        var count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
        }

        var cls = type.GetClass();
        if (cls != H5Class.String)
        {
            throw new Hdf5Exception($"Attribute is of class {cls} when expecting STRING.");
        }

        var size = GetStorageSize(attribute);

#if NET7_0_OR_GREATER
        if (size < 256)
        {
            Span<byte> buffer = stackalloc byte[size];
            read(attribute, type, buffer);
            return Encoding.ASCII.GetString(buffer);
        }
        else
        {
            var buffer = SpanOwner<byte>.Allocate(size);
            read(attribute, type, buffer.Span);
            return Encoding.ASCII.GetString(buffer.Span);
        }
#else
        using var buffer = new GlobalMemory(size + 1);
        read(attribute, type, buffer.IntPtr);
        return Marshal.PtrToStringAnsi(buffer.IntPtr, (int)size);
#endif
    }

    public static T Read<T>(H5Attribute attribute) where T : unmanaged
    {
        using var type = attribute.GetH5Type();
        using var space = attribute.GetSpace();

        long count = space.GetSimpleExtentNPoints();

        if (count != 1)
        {
            throw new Hdf5Exception("Attribute contains an array type (not supported).");
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

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static H5PropertyList GetPropertyList(H5Attribute attribute, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(attribute, get_create_plist),
            _ => throw new NotImplementedException(),
        };
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.ATTRIBUTE_CREATE),
            _ => throw new NotImplementedException(),
        };
    }
}

