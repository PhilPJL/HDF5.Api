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

using off_t = System.IntPtr;
using prp_close_func_t = HDF5.Api.NativeMethods.H5P.prp_cb1_t;
using prp_copy_func_t = HDF5.Api.NativeMethods.H5P.prp_cb1_t;
using prp_create_func_t = HDF5.Api.NativeMethods.H5P.prp_cb1_t;
using prp_delete_func_t = HDF5.Api.NativeMethods.H5P.prp_cb2_t;
using prp_get_func_t = HDF5.Api.NativeMethods.H5P.prp_cb2_t;
using prp_set_func_t = HDF5.Api.NativeMethods.H5P.prp_cb2_t;

namespace HDF5.Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5P
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Terminates access to a property list.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-Close
    /// </summary>
    /// <param name="plist">Identifier of the property list to which
    /// access is terminated.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t plist);

    /// <summary>
    /// Creates a new property list as an instance of a property list class.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-Create
    /// </summary>
    /// <param name="cls_id">The class of the property list to create.</param>
    /// <returns>Returns a property list identifier (<code>plist</code>)
    /// if successful; otherwise Fail (-1).</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pcreate"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create(hid_t cls_id);

    /// <summary>
    /// Compares two property lists or classes for equality.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-Equal
    /// </summary>
    /// <param name="id1">First property object to be compared</param>
    /// <param name="id2">Second property object to be compared</param>
    /// <returns>Returns 1 if equal; 0 if unequal. Returns a negative value
    /// on error.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pequal"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t equal(hid_t id1, hid_t id2);

    /// <summary>
    /// Retrieves the character encoding used to create a link or attribute
    /// name.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-GetCharEncoding
    /// </summary>
    /// <param name="plist_id">Link creation or attribute creation property
    /// list identifier</param>
    /// <param name="encoding">String encoding character set</param>
    /// <returns>Returns a non-negative valule if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_char_encoding"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_char_encoding
        (hid_t plist_id, ref H5T.cset_t encoding);

    /// <summary>
    /// Returns the property list class identifier for a property list.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-GetClass
    /// </summary>
    /// <param name="plist">Identifier of property list to query.</param>
    /// <returns>Returns a property list class identifier if successful.
    /// Otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_class"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_class(hid_t plist);

    /// <summary>
    /// Retrieves the name of a class.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-GetClassName
    /// </summary>
    /// <param name="pcid">Identifier of the property class to query</param>
    /// <returns>If successful returns a pointer to an allocated string
    /// containing the class name; <code>NULL</code> if unsuccessful.</returns>
    /// <remarks>The pointer to the name must be freed by the user after
    /// each successful call.</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_class_name"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial IntPtr get_class_name(hid_t pcid);

    /// <summary>
    /// Determines whether property is set to enable creating missing
    /// intermediate groups.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-GetCreateIntermediateGroup
    /// </summary>
    /// <param name="lcpl_id">Link creation property list identifier</param>
    /// <param name="crt_intermed_group">Flag specifying whether to create
    /// intermediate groups upon creation of an object</param>
    /// <returns>Returns a non-negative valule if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_create_intermediate_group"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_create_intermediate_group
        (hid_t lcpl_id, ref uint crt_intermed_group);

    /// <summary>
    /// Retrieves library version bounds settings that indirectly control
    /// the format versions used when creating objects.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-GetLibverBounds
    /// </summary>
    /// <param name="fapl_id">File access property list identifier</param>
    /// <param name="libver_low">The earliest version of the library that
    /// will be used for writing objects.</param>
    /// <param name="libver_high">The latest version of the library that
    /// will be used for writing objects.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_libver_bounds"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_libver_bounds
        (hid_t fapl_id, ref H5F.libver_t libver_low,
        ref H5F.libver_t libver_high);

    /// <summary>
    /// Sets attribute storage phase change thresholds.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetAttrPhaseChange
    /// </summary>
    /// <param name="ocpl_id">Object creation property list identifier</param>
    /// <param name="max_compact">Maximum number of attributes to be stored
    /// in compact storage</param>
    /// <param name="min_dense">Minimum number of attributes to be stored
    /// in dense storage </param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_attr_phase_change"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_attr_phase_change
        (hid_t ocpl_id, uint max_compact = 8, uint min_dense = 6);

    /// <summary>
    /// Sets the character encoding used to encode link and attribute names.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetCharEncoding
    /// </summary>
    /// <param name="plist_id">Link creation or attribute creation property
    /// list identifier</param>
    /// <param name="encoding">String encoding character set</param>
    /// <returns>Returns a non-negative valule if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_char_encoding"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_char_encoding
        (hid_t plist_id, H5T.cset_t encoding);

    /// <summary>
    /// Sets the size of the chunks used to store a chunked layout dataset.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetChunk
    /// </summary>
    /// <param name="plist_id">Dataset creation property list identifier.</param>
    /// <param name="ndims">The number of dimensions of each chunk.</param>
    /// <param name="dims">An array defining the size, in dataset elements,
    /// of each chunk.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_chunk"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_chunk
        (hid_t plist_id, int ndims,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] hsize_t[] dims);

    /// <summary>
    /// Specifies in property list whether to create missing intermediate
    /// groups.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetCreateIntermediateGroup
    /// </summary>
    /// <param name="lcpl_id">Link creation property list identifier</param>
    /// <param name="crt_intermed_group">Flag specifying whether to create
    /// intermediate groups upon the creation of an object</param>
    /// <returns>Returns a non-negative valule if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_create_intermediate_group"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_create_intermediate_group
        (hid_t lcpl_id, uint crt_intermed_group);

    /// <summary>
    /// Sets deflate (GNU gzip) compression method and compression level.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5P.html#Property-SetDeflate
    /// </summary>
    /// <param name="plist_id">Dataset or group creation property list
    /// identifier.</param>
    /// <param name="level">Compression level.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_deflate"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_deflate(hid_t plist_id, uint level);

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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_libver_bounds"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_libver_bounds
        (hid_t plist,
        H5F.libver_t low = H5F.libver_t.EARLIEST,
        H5F.libver_t high = H5F.libver_t.LATEST);
#endif
}
