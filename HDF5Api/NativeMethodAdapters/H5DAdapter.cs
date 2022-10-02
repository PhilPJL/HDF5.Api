using HDF5Api.NativeMethods;
using static HDF5Api.NativeMethods.H5D;

namespace HDF5Api.NativeMethodAdapters;

/// <summary>
/// H5 data-set native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
internal static partial class H5DAdapter
{
    public static void Close(H5DataSet dataSet)
    {
        int err = close(dataSet);

        err.ThrowIfError(nameof(close));
    }

    public static ulong GetStorageSize(H5DataSet dataSetId)
    {
        return get_storage_size(dataSetId);
    }

    public static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5PropertyList? linkCreationPropertyList = null,
        H5PropertyList? dataSetCreationPropertyList = null,
        H5PropertyList? accessCreationPropertyList = null) where T : H5Object<T>
    {
        // TODO: is this check necessary?
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = create(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(create));

        return new H5DataSet(h);
    }

    public static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = open(location, name, dataSetAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(open));

        return new H5DataSet(h);
    }

    public static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = H5L.exists(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5L.exists));

        return err > 0;
    }
    public static void SetExtent(H5DataSet dataSetId, ulong[] dimensions)
    {
        int err = set_extent(dataSetId, dimensions);

        err.ThrowIfError(nameof(set_extent));
    }

/*    internal static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer, H5PropertyList? transferPropertyList = null) where T : unmanaged
    {
        int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer);

        err.ThrowIfError(nameof(write));
    }
*/
    public static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, Span<T> buffer, H5PropertyList? transferPropertyList = null) where T : unmanaged
    {
        int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, MemoryMarshal.Cast<T, byte>(buffer));

        err.ThrowIfError(nameof(write));
    }

    public static H5Space GetSpace(H5DataSet dataSet)
    {
        long h = get_space(dataSet);

        h.ThrowIfInvalidHandleValue(nameof(get_space));

        return new H5Space(h);
    }

    public static H5Type GetType(H5DataSet dataSet)
    {
        long h = get_type(dataSet);

        h.ThrowIfInvalidHandleValue(nameof(get_type));

        return new H5Type(h);
    }
}

