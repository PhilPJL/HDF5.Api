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

    public static H5DataSet Create(H5Location location, string name, H5Type typeId, H5Space space, 
        H5PropertyList linkCreationPropertyList = default, 
        H5PropertyList dataSetCreationPropertyList = default, 
        H5PropertyList accessCreationPropertyList = default)
    {
        Handle h = H5D.create(location, name, typeId, space, H5P.DEFAULT, 
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue("H5D.create");

        return new H5DataSet(h);
    }

    public static H5DataSet Open(H5Location location, string name)
    {
        location.ThrowIfNotValid();

        Handle h = H5D.open(location, name);

        h.ThrowIfInvalidHandleValue("H5D.open");

        return new H5DataSet(h);
    }

    public static bool Exists(H5Location location, string name)
    {
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
        Handle h = H5D.get_space(dataSet);

        h.ThrowIfInvalidHandleValue("H5D.get_space");

        return new H5Space(h);
    }
}

