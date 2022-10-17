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
internal static partial class H5L
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Determine whether a link with the specified name exists in a group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5L.html#Link-Exists
    /// </summary>
    /// <param name="loc_id">Identifier of the file or group to query.</param>
    /// <param name="name">The name of the link to check.</param>
    /// <param name="lapl_id">Link access property list identifier.</param>
    /// <returns>Returns 1 or 0 if successful; otherwise returns a negative
    /// value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lexists", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t exists(hid_t loc_id, string name,
        hid_t lapl_id = H5P.DEFAULT);

    /// <summary>
    /// Removes a link from a group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5L.html#Link-Delete
    /// </summary>
    /// <param name="loc_id">Identifier of the file or group containing
    /// the object.</param>
    /// <param name="name">Name of the link to delete.</param>
    /// <param name="lapl_id">Link access property list identifier.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ldelete", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t delete(hid_t loc_id, string name,
        hid_t lapl_id = H5P.DEFAULT);

    /// <summary>
    /// Iterates through links in a group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5L.html#Link-Iterate
    /// </summary>
    /// <param name="grp_id">Identifier specifying subject group</param>
    /// <param name="idx_type">Type of index which determines the order</param>
    /// <param name="order">Order within index</param>
    /// <param name="idx">Iteration position at which to start</param>
    /// <param name="op">Callback function passing data regarding the link
    /// to the calling application</param>
    /// <param name="op_data">User-defined pointer to data required by the
    /// application for its processing of the link</param>
    /// <returns>On success, returns the return value of the first operator
    /// that returns a positive value, or zero if all members were
    /// processed with no operator returning non-zero. On failure, returns
    /// a negative value if something goes wrong within the library, or the
    /// first negative value returned by an operator.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Literate"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t iterate
        (hid_t grp_id, H5.index_t idx_type, H5.iter_order_t order,
        ref hsize_t idx, iterate_t op, IntPtr op_data);
#endif
}