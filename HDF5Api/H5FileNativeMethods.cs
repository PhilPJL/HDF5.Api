
using HDF5Api.NativeMethods;

namespace HDF5Api;

/// <summary>
/// H5 file native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_f.html"/>
/// </summary>
internal static partial class H5FileNativeMethods
{
    #region Close

    /// <summary>
    /// Terminates access to an HDF5 file.
    /// </summary>
    /// <param name="file_id">Identifier of a file to which access is
    /// terminated.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Fclose(hid_t file_id);

    public static void Close(H5File file)
    {
        int err = H5Fclose(file);

        err.ThrowIfError(nameof(H5Fclose));
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates an HDF5 file.
    /// </summary>
    /// <param name="filename">Name of the file to access.</param>
    /// <param name="flags">File access flags (H5.ACC_*).</param>
    /// <param name="create_plist">File creation property list identifier.
    /// </param>
    /// <param name="access_plist">File access property list identifier.
    /// </param>
    /// <returns>Returns a file identifier if successful; otherwise returns
    /// a negative value.</returns>
    /// <remarks><paramref name="filename"/> MUST be an ASCII string.</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fcreate", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Fcreate(string filename, uint flags, hid_t create_plist, hid_t access_plist);

    /// <summary>
    ///     Create a new file.
    /// </summary>
    public static H5File Create(string path, H5PropertyList? fileCreationPropertyList = null, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = H5Fcreate(path, (uint)H5FileOptions.Truncate, fileCreationPropertyList, fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Fcreate));

        return new H5File(h);
    }

    #endregion

    #region Open

    /// <summary>
    /// Opens an existing HDF5 file.
    /// </summary>
    /// <param name="filename">Name of the file to be opened.</param>
    /// <param name="flags">File access flags. (<code>H5F_ACC_RDWR</code>
    /// or <code>H5F_ACC_RDONLY</code>)</param>
    /// <param name="plist">Identifier for the file access properties
    /// list.</param>
    /// <returns>Returns a file identifier if successful; otherwise returns
    /// a negative value.</returns>
    /// <remarks><paramref name="filename"/> MUST be an ASCII string!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fopen", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Fopen(string filename, uint flags, hid_t plist);

    public static H5File Open(string path, bool readOnly, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = H5Fopen(path, (uint)(readOnly ? H5FileOptions.ReadOnly : H5FileOptions.ReadWrite), fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Fopen));

        return new H5File(h);
    }

    #endregion

    #region GetObjectCount

    /// <summary>
    /// Returns the number of open object identifiers for an open file.
    /// </summary>
    /// <param name="file_id">Identifier of a currently-open HDF5 file.
    /// </param>
    /// <param name="types">Type of object for which identifiers are to be
    /// returned.</param>
    /// <returns>Returns the number of open objects if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_obj_count")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ssize_t H5Fget_obj_count(hid_t file_id, uint types);

    public static long GetObjectCount(H5File file, H5ObjectTypes types = H5ObjectTypes.All)
    {
        return (long)H5Fget_obj_count(file, (uint)types);
    }

    #endregion
}

internal enum H5FileOptions : uint
{
    // Flags for H5F.open() and H5F.create() calls

    /// <summary>
    /// absence of rdwr => rd-only
    /// </summary>
    ReadOnly = 0x0000u,

    /// <summary>
    /// open for read and write
    /// </summary>
    ReadWrite = 0x0001u,

    /// <summary>
    /// overwrite existing files
    /// </summary>
    Truncate = 0x0002u,

    /// <summary>
    /// fail if file already exists
    /// </summary>
    Exclusive = 0x0004u,

    /// <summary>
    /// create non-existing files
    /// </summary>
    Create = 0x0010u,

    /// <summary>
    /// indicate that this file is open for writing in a
    /// single-writer/multi-reader (SWMR) scenario.  Note that the
    /// process(es) opening the file for reading must open the file
    /// with <code>RDONLY</code> access, and use the special
    /// <code>SWMR_READ</code> access flag.
    /// </summary>
    SingleWriteMultiReadWrite = 0x0020u,

    /// <summary>
    /// indicate that this file is open for reading in a 
    /// single-writer/multi-reader (SWMR) scenario. Note that the
    /// process(es) opening the file for SWMR reading must also
    /// open the file with the <code>RDONLY</code> flag.
    /// </summary>
    SingleWriteMultiReadRead = 0x0040u
}