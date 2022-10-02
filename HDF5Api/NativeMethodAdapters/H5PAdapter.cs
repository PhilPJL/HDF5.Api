using HDF5Api.NativeMethods;

namespace HDF5Api.NativeMethodAdapters;

internal static partial class H5PAdapter
{
    #region Close

    /// <summary>
    /// Terminates access to a property list.
    /// </summary>
    /// <param name="plist">Identifier of the property list to which
    /// access is terminated.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Pclose(hid_t plist);

    public static void Close(H5PropertyList propertyList)
    {
        int err = H5Pclose(propertyList);

        err.ThrowIfError(nameof(H5Pclose));
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates a new property list as an instance of a property list class.
    /// </summary>
    /// <param name="cls_id">The class of the property list to create.</param>
    /// <returns>Returns a property list identifier (<code>plist</code>)
    /// if successful; otherwise Fail (-1).</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pcreate")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Pcreate(hid_t cls_id);

    public static H5PropertyList Create(long classId)
    {
        long h = H5Pcreate(classId);

        h.ThrowIfInvalidHandleValue(nameof(H5Pcreate));

        return new H5PropertyList(h);
    }

    #endregion

    #region SetChunk

    /// <summary>
    /// Sets the size of the chunks used to store a chunked layout dataset.
    /// </summary>
    /// <param name="plist_id">Dataset creation property list identifier.</param>
    /// <param name="ndims">The number of dimensions of each chunk.</param>
    /// <param name="dims">An array defining the size, in dataset elements,
    /// of each chunk.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_chunk")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Pset_chunk(hid_t plist_id, int ndims, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] hsize_t[] dims);

    public static void SetChunk(H5PropertyList propertyList, int rank, ulong[] dims)
    {
        int err = H5Pset_chunk(propertyList, rank, dims);

        err.ThrowIfError(nameof(H5Pset_chunk));
    }

    #endregion

    #region EnableDeflateCompression

    /// <summary>
    /// Sets deflate (GNU gzip) compression method and compression level.
    /// </summary>
    /// <param name="plist_id">Dataset or group creation property list
    /// identifier.</param>
    /// <param name="level">Compression level.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_deflate")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Pset_deflate(hid_t plist_id, uint level);

    public static void EnableDeflateCompression(H5PropertyList propertyList, uint level)
    {
        int err = H5Pset_deflate(propertyList, level);

        err.ThrowIfError(nameof(H5Pset_deflate));
    }

    #endregion

    #region GetCreationPropertyList

    /// <summary>
    /// Returns an identifier for a copy of the dataset creation property
    /// list for a dataset.
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a dataset creation property list identifier if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_create_plist")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Dget_create_plist(hid_t dset_id);

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    public static H5PropertyList GetCreationPropertyList(H5DataSet dataSet)
    {
        long h = H5D.get_create_plist(dataSet);
        h.ThrowIfInvalidHandleValue(nameof(H5Dget_create_plist));
        return new H5PropertyList(h);
    }

    #endregion
}