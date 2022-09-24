using System;
using System.Runtime.InteropServices;

namespace HDF5Api;

internal static partial class H5DataSetNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dclose")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int H5DClose(long handle);

    public static void Close(H5DataSet dataSet)
    {
        int err = H5DClose(dataSet);

        // TODO: get additional error info 
        err.ThrowIfError("H5Dclose");
    }

    #endregion

    public static long GetStorageSize(long dataSetId)
    {
        dataSetId.AssertIsHandleType(HandleType.DataSet);

        return (long)H5D.get_storage_size(dataSetId);
    }

    public static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5PropertyList? linkCreationPropertyList = null,
        H5PropertyList? dataSetCreationPropertyList = null,
        H5PropertyList? accessCreationPropertyList = null) where T : H5Object<T>
    {
        location.Handle.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5D.create(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue("H5D.create");

        return new H5DataSet(h);
    }

    public static H5DataSet Open<T>(H5Location<T> location, string name) where T : H5Object<T>
    {
        location.Handle.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5D.open(location, name);

        h.ThrowIfInvalidHandleValue("H5D.open");

        return new H5DataSet(h);
    }

    public static bool Exists<T>(H5Location<T> location, string name) where T : H5Object<T>
    {
        location.Handle.AssertIsHandleType(HandleType.File, HandleType.Group);

        int err = H5L.exists(location, name);

        err.ThrowIfError("H5L.exists");

        return err > 0;
    }

    public static void SetExtent(H5DataSet dataSetId, ulong[] dims)
    {
        int err = H5D.set_extent(dataSetId, dims);

        err.ThrowIfError("H5D.set_extent");
    }

    public static void Write(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer)
    {
        int err = H5D.write(dataSet, type, memorySpace, fileSpace, default, buffer);

        err.ThrowIfError("H5D.write");
    }

    public static H5Space GetSpace(H5DataSet dataSet)
    {
        long h = H5D.get_space(dataSet);

        h.ThrowIfInvalidHandleValue("H5D.get_space");

        return new H5Space(h);
    }
}

