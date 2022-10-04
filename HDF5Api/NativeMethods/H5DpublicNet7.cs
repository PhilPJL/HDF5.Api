/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace HDF5Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5D
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Closes the specified dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-Close
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to close access to.
    /// </param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t dset_id);

    /// <summary>
    /// Creates a new dataset and links it into the file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-Create2
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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dcreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create
        (hid_t loc_id, string name, hid_t type_id, hid_t space_id,
        hid_t lcpl_id = H5P.DEFAULT, hid_t dcpl_id = H5P.DEFAULT,
        hid_t dapl_id = H5P.DEFAULT);

    /// <summary>
    /// Returns the dataset access property list associated with a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-GetAccessPlist
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to get access
    /// property list of.</param>
    /// <returns>Returns a dataset access property list identifier if
    /// Ssuccessful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_access_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_access_plist(hid_t dset_id);

    /// <summary>
    /// Returns an identifier for a copy of the dataset creation property
    /// list for a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-GetCreatePlist
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a dataset creation property list identifier if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_create_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_create_plist(hid_t dset_id);

    /// <summary>
    /// Returns an identifier for a copy of the dataspace for a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-GetSpace
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a dataspace identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_space"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_space(hid_t dset_id);

    /// <summary>
    /// Returns the amount of storage allocated for a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-GetStorageSize
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns the amount of storage space, in bytes, allocated
    /// for the dataset, not counting metadata; otherwise returns 0 (zero).
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_storage_size"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hsize_t get_storage_size(hid_t dset_id);

    /// <summary>
    /// Returns an identifier for a copy of the datatype for a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-GetType
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset to query.</param>
    /// <returns>Returns a datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dget_type"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_type(hid_t dset_id);

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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dopen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t open
        (hid_t file_id, string name, hid_t dapl_id = H5P.DEFAULT);

    /// <summary>
    /// Reads raw data from a dataset into a buffer.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-Read
    /// </summary>
    /// <param name="dset_id">Identifier of the dataset read from.</param>
    /// <param name="mem_type_id">Identifier of the memory datatype.</param>
    /// <param name="mem_space_id">Identifier of the memory dataspace.</param>
    /// <param name="file_space_id">Identifier of the dataset's dataspace
    /// in the file.</param>
    /// <param name="plist_id">Identifier of a transfer property list for
    /// this I/O operation.</param>
    /// <param name="buf">Buffer to receive data read from file.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dread"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t read
        (hid_t dset_id, hid_t mem_type_id, hid_t mem_space_id,
        hid_t file_space_id, hid_t plist_id, IntPtr buf);

    /// <summary>
    /// Changes the sizes of a dataset’s dimensions.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-SetExtent
    /// </summary>
    /// <param name="dset_id">Dataset identifier</param>
    /// <param name="size">Array containing the new magnitude of each
    /// dimension of the dataset.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dset_extent"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_extent(hid_t dset_id, hsize_t[] size);

    /// <summary>
    /// Writes raw data from a buffer to a dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5D.html#Dataset-Write
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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Dwrite"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t write
        (hid_t dset_id, hid_t mem_type_id, hid_t mem_space_id,
        hid_t file_space_id, hid_t plist_id, IntPtr buf);
#endif
}
