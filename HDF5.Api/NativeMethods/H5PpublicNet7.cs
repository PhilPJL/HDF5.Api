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
#endif
}
