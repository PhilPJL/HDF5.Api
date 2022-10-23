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

namespace HDF5.Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5F
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Terminates access to an HDF5 file.
    /// https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-Close
    /// </summary>
    /// <param name="file_id">Identifier of a file to which access is
    /// terminated.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t file_id);

    /// <summary>
    /// Creates an HDF5 file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-CreateS
    /// </summary>
    /// <param name="filename">Name of the file to access.</param>
    /// <param name="flags">File access flags (H5.ACC_*).</param>
    /// <param name="create_plist">File creation property list identifier.
    /// </param>
    /// <param name="access_plist">File access property list identifier.
    /// </param>
    /// <returns>Returns a file identifier if successful; otherwise returns
    /// a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fcreate", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create
        (string filename, uint flags,
        hid_t create_plist = H5P.DEFAULT, hid_t access_plist = H5P.DEFAULT);

    /// <summary>
    /// Flushes all buffers associated with a file to disk.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-Flush
    /// </summary>
    /// <param name="object_id">Identifier of object used to identify the
    /// file.</param>
    /// <param name="scope">Specifies the scope of the flushing
    /// action.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fflush"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t flush(hid_t object_id, scope_t scope);

    /// <summary>
    /// Returns a file access property list identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-GetAccessPlist
    /// </summary>
    /// <param name="file_id">Identifier of file of which to get access
    /// property list</param>
    /// <returns>Returns a file access property list identifier if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_access_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_access_plist(hid_t file_id);

    /// <summary>
    /// Returns a file creation property list identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-GetCreatePlist
    /// </summary>
    /// <param name="file_id">Identifier of file of which to get creation
    /// property list</param>
    /// <returns>Returns a file access property list identifier if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_create_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_create_plist(hid_t file_id);

    /// <summary>
    /// Returns the size of an HDF5 file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-GetFilesize
    /// </summary>
    /// <param name="file_id">Identifier of a currently-open HDF5
    /// file</param>
    /// <param name="size">Size of the file, in bytes.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_filesize"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_filesize
        (hid_t file_id, ref hsize_t size);

    /// <summary>
    /// Returns the number of open object identifiers for an open file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-GetObjCount
    /// </summary>
    /// <param name="file_id">Identifier of a currently-open HDF5 file.
    /// </param>
    /// <param name="types">Type of object for which identifiers are to be
    /// returned.</param>
    /// <returns>Returns the number of open objects if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_obj_count"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial ssize_t get_obj_count
        (hid_t file_id, uint types);

    /// <summary>
    /// Retrieves name of file to which object belongs.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-GetName
    /// </summary>
    /// <param name="obj_id">Identifier of the object for which the
    /// associated filename is sought.</param>
    /// <param name="name">Buffer to contain the returned filename.</param>
    /// <param name="size">Buffer size, in bytes.</param>
    /// <returns>Returns the length of the filename if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_name"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial nint get_name(hid_t obj_id, Span<byte> name, nint size);

    /// <summary>
    /// Opens an existing HDF5 file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5F.html#File-Open
    /// </summary>
    /// <param name="filename">Name of the file to be opened.</param>
    /// <param name="flags">File access flags. (<code>H5F_ACC_RDWR</code>
    /// or <code>H5F_ACC_RDONLY</code>)</param>
    /// <param name="plist">Identifier for the file access properties
    /// list.</param>
    /// <returns>Returns a file identifier if successful; otherwise returns
    /// a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fopen", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t open
        (string filename, uint flags, hid_t plist = H5P.DEFAULT);
         
    /// <summary>
    /// Sets bounds on library versions, and indirectly format versions,
    /// to be used when creating objects.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetLibverBounds
    /// </summary>
    /// <param name="plist">File access property list identifier</param>
    /// <param name="low">The earliest version of the library that will be
    /// used for writing objects, indirectly specifying the earliest object
    /// format version that can be used when creating objects in the file.
    /// </param>
    /// <param name="high">The latest version of the library that will be
    /// used for writing objects, indirectly specifying the latest object
    /// format version that can be used when creating objects in the file.
    /// </param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fset_libver_bounds"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_libver_bounds
        (hid_t file_id,
        H5F.libver_t low = H5F.libver_t.EARLIEST,
        H5F.libver_t high = H5F.libver_t.LATEST);

#endif
}