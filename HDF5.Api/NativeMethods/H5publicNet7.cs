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
internal static partial class H5
{
#if NET7_0_OR_GREATER

    /// <summary>
    /// Frees memory allocated by the HDF5 Library.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5.html#Library-FreeMemory
    /// </summary>
    /// <param name="buf">
    /// Buffer to be freed. Can be NULL.
    /// </param>
    /// <returns>
    /// Returns a non-negative value if successful; otherwise returns a
    /// negative value.
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5free_memory"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t free_memory(IntPtr buf);

    /// <summary>
    /// Returns the HDF library release number.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5.html#Library-Version
    /// </summary>
    /// <param name="majnum">
    /// The major version of the library.
    /// </param>
    /// <param name="minnum">
    /// The minor version of the library.
    /// </param>
    /// <param name="relnum">
    /// The release number of the library.
    /// </param>
    /// <returns>
    /// Returns a non-negative value if successful; otherwise returns a
    /// negative value.
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5get_libversion"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_libversion
        (ref uint majnum, ref uint minnum, ref uint relnum);

    /// <summary>
    /// Determine whether the HDF5 Library was built with the thread-safety
    /// feature enabled.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5.html#Library-IsLibraryThreadsafe
    /// </summary>
    /// <param name="is_ts">
    /// Boolean value indicating whether the library was built with
    /// thread-safety enabled.
    /// </param>
    /// <returns>
    /// Returns a non-negative value if successful; otherwise returns a
    /// negative value.
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5is_library_threadsafe"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t is_library_threadsafe(ref hbool_t is_ts);

    /// <summary>
    /// Initializes the HDF5 library.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5.html#Library-Open
    /// </summary>
    /// <returns>
    /// Returns a non-negative value if successful; otherwise returns a
    /// negative value.
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5open"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t open();
#endif
}