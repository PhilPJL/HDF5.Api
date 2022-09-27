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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Dclose(hid_t dset_id);

    public static void Close(H5DataSet dataSet)
    {
        int err = H5Dclose(dataSet);

        err.ThrowIfError(nameof(H5Dclose));
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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hsize_t H5Dget_storage_size(hid_t dset_id);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Dcreate2(hid_t loc_id, string name, hid_t type_id, hid_t space_id, hid_t lcpl_id, hid_t dcpl_id, hid_t dapl_id);

    public static H5DataSet Create<T>(H5Location<T> location, string name, H5Type type, H5Space space,
        H5PropertyList? linkCreationPropertyList = null,
        H5PropertyList? dataSetCreationPropertyList = null,
        H5PropertyList? accessCreationPropertyList = null) where T : H5Object<T>
    {
        // TODO: is this check necessary?
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Dcreate2(location, name, type, space,
            linkCreationPropertyList, dataSetCreationPropertyList, accessCreationPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Dcreate2));

        return new H5DataSet(h);
    }

    #endregion

    #region Open

    /// <summary>
    /// Opens an existing dataset.
    /// </summary>
    /// <param name="file_id">Location identifier</param>
    /// <param name="name">Dataset name</param>
    /// <param name="dapl_id">Dataset access property list</param>
    /// <returns>Returns a dataset identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dopen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Dopen2(hid_t file_id, string name, hid_t dapl_id);

    public static H5DataSet Open<T>(H5Location<T> location, string name, H5PropertyList? dataSetAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Dopen2(location, name, dataSetAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Dopen2));

        return new H5DataSet(h);
    }

    #endregion

    #region Exists

    /// <summary>
    /// Determine whether a link with the specified name exists in a group.
    /// </summary>
    /// <param name="loc_id">Identifier of the file or group to query.</param>
    /// <param name="name">The name of the link to check.</param>
    /// <param name="lapl_id">Link access property list identifier.</param>
    /// <returns>Returns 1 or 0 if successful; otherwise returns a negative
    /// value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lexists", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial htri_t H5Lexists(hid_t loc_id, string name, hid_t lapl_id);

    public static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = H5Lexists(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5Lexists));

        return err > 0;
    }

    #endregion

    #region SetExtent

    /// <summary>
    /// Changes the sizes of a dataset’s dimensions.
    /// </summary>
    /// <param name="dset_id">Dataset identifier</param>
    /// <param name="size">Array containing the new magnitude of each
    /// dimension of the dataset.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dset_extent")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Dset_extent(hid_t dset_id, hsize_t[] size);

    public static void SetExtent(H5DataSet dataSetId, ulong[] dimensions)
    {
        int err = H5Dset_extent(dataSetId, dimensions);

        err.ThrowIfError("H5D.set_extent");
    }

    #endregion

    #region Write

    /// <summary>
    /// Writes raw data from a buffer to a dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to write to.</param>
    /// <param name="mem_type_id">Identifier of the memory datatype.</param>
    /// <param name="mem_space_id">Identifier of the memory dataspace.</param>
    /// <param name="file_space_id">Identifier of the dataset's dataspace
    /// in the file.</param>
    /// <param name="plist_id">Identifier of a transfer property list for
    /// this I/O operation.</param>
    /// <param name="buf">Buffer with data to be written to the file.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dwrite")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Dwrite
        (hid_t dset_id, hid_t mem_type_id, hid_t mem_space_id, hid_t file_space_id, hid_t plist_id, IntPtr buf);

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dwrite")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Dwrite
        (hid_t dset_id, hid_t mem_type_id, hid_t mem_space_id, hid_t file_space_id, hid_t plist_id, Span<byte> buf);

    public static void Write(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, IntPtr buffer, H5PropertyList? transferPropertyList = null)
    {
        int err = H5Dwrite(dataSet, type, memorySpace, fileSpace, transferPropertyList, buffer);

        err.ThrowIfError("H5D.write");
    }

    public static void Write<T>(H5DataSet dataSet, H5Type type, H5Space memorySpace, H5Space fileSpace, Span<T> buffer, H5PropertyList? transferPropertyList = null) where T : unmanaged
    {
        int err = H5Dwrite(dataSet, type, memorySpace, fileSpace, transferPropertyList, MemoryMarshal.Cast<T, byte>(buffer));

        err.ThrowIfError("H5D.write");
    }

    #endregion

    #region GetSpace

    /// <summary>
    /// Returns an identifier for a copy of the dataspace for a dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a dataspace identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_space")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Dget_space(hid_t dset_id);

    public static H5Space GetSpace(H5DataSet dataSet)
    {
        long h = H5Dget_space(dataSet);

        h.ThrowIfInvalidHandleValue(nameof(H5Dget_space));

        return new H5Space(h);
    }

    #endregion

    #region GetType

    /// <summary>
    /// Returns an identifier for a copy of the datatype for a dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_type")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial hid_t H5Dget_type(hid_t dset_id);

    public static H5Type GetType(H5DataSet dataSet)
    {
        long h = H5Dget_type(dataSet);

        h.ThrowIfInvalidHandleValue(nameof(H5Dget_type));

        return new H5Type(h);
    }

    #endregion
}

