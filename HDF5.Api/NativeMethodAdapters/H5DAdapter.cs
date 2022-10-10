using HDF5.Api.NativeMethods;
using System.Linq;
using static HDF5.Api.NativeMethods.H5D;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 data-set native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
internal static class H5DAdapter
{
    public static void Close(H5DataSet dataSet)
    {
        int err = close(dataSet);

        err.ThrowIfError();
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

        h.ThrowIfInvalidHandleValue();

        return new H5DataSet(h);
    }

    public static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = open(location, name, dataSetAccessPropertyList);

        h.ThrowIfInvalidHandleValue();

        return new H5DataSet(h);
    }

    public static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        return H5LAdapter.Exists(location, name, linkAccessPropertyList);
    }

    public static void SetExtent(H5DataSet dataSetId, params long[] dimensions)
    {
        int err = set_extent(dataSetId, dimensions.Select(d => (ulong)d).ToArray());

        err.ThrowIfError();
    }

    internal static void Write(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer, H5PropertyList? transferPropertyList = null) 
    {
        int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer);

        err.ThrowIfError();
    }

/*    public static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, Span<T> buffer, H5PropertyList? transferPropertyList = null) where T : unmanaged
    {
        int err = write(dataSet, type, memorySpace, fileSpace, transferPropertyList, MemoryMarshal.Cast<T, byte>(buffer));

        err.ThrowIfError(nameof(write));
    }*/

    public static H5Space GetSpace(H5DataSet dataSet)
    {
        long h = get_space(dataSet);

        h.ThrowIfInvalidHandleValue();

        return new H5Space(h);
    }

    public static H5Type GetType(H5DataSet dataSet)
    {
        long h = get_type(dataSet);

        h.ThrowIfInvalidHandleValue();

        return new H5Type(h);
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.Create(H5P.DATASET_CREATE),
            PropertyListType.Access => H5PAdapter.Create(H5P.DATASET_ACCESS),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    public static H5PropertyList GetPropertyList(H5DataSet dataSet, PropertyListType listType)
    {
        return listType switch
        {
            PropertyListType.Create => H5PAdapter.GetPropertyList(dataSet, get_create_plist),
            PropertyListType.Access => H5PAdapter.GetPropertyList(dataSet, get_access_plist),
            _ => throw new NotImplementedException(),
        };
    }
}

