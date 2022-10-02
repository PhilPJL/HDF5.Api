using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

using HDF5Api.NativeMethods;
using H5A_NM = HDF5Api.NativeMethods.H5A;

namespace HDF5Api.NativeMethodAdapters;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal static partial class H5AAdapter
{
    public static void Close(H5Attribute attribute)
    {
        int err = H5A_NM.close(attribute);

        err.ThrowIfError(nameof(H5A_NM.close));
    }

    public static H5Attribute Create<T>(H5Object<T> h5Object, string name, H5Type type, H5Space space,
        H5PropertyList? creationPropertyList = null, H5PropertyList? accessPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        var h = H5A_NM.create(h5Object, name, type, space, creationPropertyList, accessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5A_NM.create));

        return new H5Attribute(h);
    }

    public static H5Attribute Open<T>(H5Object<T> h5Object, string name, H5PropertyList? attributeAccessPropertyList = default) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        long h = H5A_NM.open(h5Object, name, attributeAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5A_NM.open));

        return new H5Attribute(h);
    }

    public static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5A_NM.delete(h5Object, name);

        err.ThrowIfError(nameof(H5A_NM.delete));
    }

    public static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5A_NM.exists(h5Object, name);

        err.ThrowIfError(nameof(H5A_NM.exists));
        return err > 0;
    }

    public static IEnumerable<string> ListAttributeNames<T>(H5Object<T> h5Object, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = H5A_NM.iterate(h5Object,
            H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

        err.ThrowIfError(nameof(H5A_NM.iterate));

        return names;

        int Callback(long id, IntPtr intPtrName, ref H5A_NM.info_t info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);

            Guard.IsNotNull(name);

            var ainfo = GetInfoByName(h5Object, ".", name, linkAccessPropertyList);

            names.Add(name);
            return 0;
        }
    }

    // TODO: make public?
    private static H5A_NM.info_t GetInfoByName<T>(H5Object<T> h5Object,
        string objectName, string attributeName, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        H5A_NM.info_t info = default;
        int err = H5A_NM.get_info_by_name(h5Object, objectName, attributeName, ref info, linkAccessPropertyList);
        err.ThrowIfError(nameof(H5A_NM.get_info_by_name));
        return info;
    }

    public static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = H5A_NM.write(attribute, type, buffer);

        err.ThrowIfError(nameof(H5A_NM.write));
    }

    public static H5Space GetSpace(H5Attribute attribute)
    {
        var space = H5A_NM.get_space(attribute);

        space.ThrowIfError(nameof(H5A_NM.get_space));

        return new H5Space(space);
    }

    public static long GetStorageSize(H5Attribute attribute)
    {
        return (long)H5A_NM.get_storage_size(attribute);
    }

    public static H5Type GetType(H5Attribute attribute)
    {
        long typeHandle = H5A_NM.get_type(attribute);
        typeHandle.ThrowIfInvalidHandleValue(nameof(H5A_NM.get_type));
        return new H5Type(typeHandle);
    }

    public static void Read<T>(H5Attribute attribute, H5Type type, Span<T> buffer) where T : unmanaged
    {
        int err = H5A_NM.read(attribute, type, MemoryMarshal.AsBytes(buffer));

        err.ThrowIfError(nameof(H5A_NM.read));
    }
}

