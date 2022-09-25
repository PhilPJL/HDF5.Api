using System;

namespace HDF5Api;

/// <summary>
/// H5 data-set native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
internal static partial class H5DataSetNativeMethods
{
    #region Close

    /// <summary>
    /// Closes the specified dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to close access to.
    /// </param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dclose")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Dclose(long dset_id);

    public static void Close(H5DataSet dataSet)
    {
        int err = H5Dclose(dataSet);

        err.ThrowIfError("H5Dclose");
    }

    #endregion

    #region GetStorageSize

    /// <summary>
    /// Returns the amount of storage allocated for a dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns the amount of storage space, in bytes, allocated
    /// for the dataset, not counting metadata; otherwise returns 0 (zero).
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_storage_size")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial ulong H5Dget_storage_size(long dset_id);

    public static long GetStorageSize(H5DataSet dataSetId)
    {
        return (long)H5Dget_storage_size(dataSetId);
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates a new dataset and links it into the file.
    /// </summary>
    /// <param name="loc_id">Location identifier</param>
    /// <param name="name">Dataset name</param>
    /// <param name="type_id">Datatype identifier</param>
    /// <param name="space_id">Dataspace identifier</param>
    /// <param name="lcpl_id">Link creation property list</param>
    /// <param name="dcpl_id">Dataset creation property list</param>
    /// <param name="dapl_id">Dataset access property list</param>
    /// <returns>Returns a dataset identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dcreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial long H5Dcreate2
        (long loc_id, string name, long type_id, long space_id, long lcpl_id, long dcpl_id, long dapl_id);

    public static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5PropertyList? linkCreationPropertyList = null,
        H5PropertyList? dataSetCreationPropertyList = null,
        H5PropertyList? accessCreationPropertyList = null) where T : H5Object<T>
    {
        // TODO: is this check necessary?
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Dcreate2(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue("H5Dcreate2");

        return new H5DataSet(h);
    }

    #endregion

    #region Open

    /// <summary>
    /// Opens an existing dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-Open2
    /// </summary>
    /// <param name="file_id">Location identifier</param>
    /// <param name="name">Dataset name</param>
    /// <param name="dapl_id">Dataset access property list</param>
    /// <returns>Returns a dataset identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dopen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial long H5Dopen2(long file_id, string name, long dapl_id);

    public static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Dopen2(location, name, dataSetAccessPropertyList);

        h.ThrowIfInvalidHandleValue("H5Dopen2");

        return new H5DataSet(h);
    }

    #endregion

    public static bool Exists<T>(H5Location<T> location, string name) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

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

    public static H5Type GetType(H5DataSet dataSet)
    {
        long h = H5D.get_type(dataSet);

        h.ThrowIfInvalidHandleValue("H5D.get_type");

        return new H5Type(h);
    }
}

