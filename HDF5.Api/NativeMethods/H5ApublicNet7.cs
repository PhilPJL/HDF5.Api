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
internal static partial class H5A
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Closes the specified attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Close
    /// </summary>
    /// <param name="attr_id">Attribute to release access to.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t attr_id);

    /// <summary>
    /// Creates an attribute attached to a specified object.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Create2
    /// </summary>
    /// <param name="loc_id">Location or object identifier</param>
    /// <param name="attr_name">Attribute name</param>
    /// <param name="type_id">Attribute datatype identifier</param>
    /// <param name="space_id">Attribute dataspace identifier</param>
    /// <param name="acpl_id">Attribute creation property list identifier</param>
    /// <param name="aapl_id">Attribute access property list identifier</param>
    /// <returns>Returns an attribute identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Acreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create
        (hid_t loc_id, string attr_name, hid_t type_id, hid_t space_id,
        hid_t acpl_id = H5P.DEFAULT, hid_t aapl_id = H5P.DEFAULT);

    /// <summary>
    /// Deletes an attribute from a specified location.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Delete
    /// </summary>
    /// <param name="loc_id">Identifier of the dataset, group, or named
    /// datatype to have the attribute deleted from.</param>
    /// <param name="name">Name of the attribute to delete.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Adelete", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t delete(hid_t loc_id, string name);

    /// <summary>
    /// Determines whether an attribute with a given name exists on an object.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Exists
    /// </summary>
    /// <param name="obj_id">Object identifier</param>
    /// <param name="attr_name">Attribute name</param>
    /// <returns>When successful, returns a positive value, for
    /// <code>TRUE</code>, or 0 (zero), for <code>FALSE</code>. Otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aexists", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t exists(hid_t obj_id, string attr_name);

    /// <summary>
    /// Gets an attribute creation property list identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetCreatePlist
    /// </summary>
    /// <param name="attr_id">Identifier of the attribute.</param>
    /// <returns>Returns an identifier for the attribute’s creation property
    /// list if successful. Otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_create_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_create_plist(hid_t attr_id);

    /// <summary>
    /// Retrieves attribute information, by attribute name.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetInfoByName
    /// </summary>
    /// <param name="loc_id">Location of object to which attribute is
    /// attached</param>
    /// <param name="obj_name">Name of object to which attribute is
    /// attached, relative to location</param>
    /// <param name="attr_name">Attribute name</param>
    /// <param name="ainfo">Struct containing returned attribute
    /// information</param>
    /// <param name="lapl_id">Link access property list</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_info_by_name", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_info_by_name
        (hid_t loc_id, string obj_name, string attr_name,
        ref info_t ainfo, hid_t lapl_id = H5P.DEFAULT);

    /// <summary>
    /// Gets an attribute name.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetName
    /// </summary>
    /// <param name="attr_id">Identifier of the attribute.</param>
    /// <param name="size">The size of the buffer to store the name
    /// in.</param>
    /// <param name="name">Buffer to store name in.</param>
    /// <returns>Returns the length of the attribute's name, which may be
    /// longer than <code>buf_size</code>, if successful. Otherwise returns
    /// a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_name"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial nint get_name(hid_t attr_id, nint size, Span<byte> name);

    /// <summary>
    /// Gets a copy of the dataspace for an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetSpace
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute.</param>
    /// <returns>Returns attribute dataspace identifier if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_space"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_space(hid_t attr_id);

    /// <summary>
    /// Returns the amount of storage required for an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetStorageSize
    /// </summary>
    /// <param name="attr_id">Identifier of the attribute to query.</param>
    /// <returns>Returns the amount of storage size allocated for the
    /// attribute; otherwise returns 0 (zero).</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_storage_size"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hsize_t get_storage_size(hid_t attr_id);

    /// <summary>
    /// Returns the amount of storage required for an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetType
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute.</param>
    /// <returns>Returns a datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_type"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_type(hid_t attr_id);

    /// <summary>
    /// Calls user-defined function for each attribute on an object.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Iterate2
    /// </summary>
    /// <param name="obj_id">Identifier for object to which attributes are
    /// attached; may be group, dataset, or named datatype.</param>
    /// <param name="idx_type">Type of index</param>
    /// <param name="order">Order in which to iterate over index</param>
    /// <param name="n">Initial and returned offset within index</param>
    /// <param name="op">User-defined function to pass each attribute to</param>
    /// <param name="op_data">User data to pass through to and to be
    /// returned by iterator operator function</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value. Further note that this function returns
    /// the return value of the last operator if it was non-zero, which
    /// can be a negative value, zero if all attributes were processed, or
    /// a positive value indicating short-circuit success
    /// </returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aiterate2"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t iterate
        (hid_t obj_id, H5.index_t idx_type, H5.iter_order_t order,
        ref hsize_t n, operator_t op, IntPtr op_data);

    /// <summary>
    /// Opens an attribute for an object specified by object identifier
    /// and attribute name.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Open
    /// </summary>
    /// <param name="obj_id">Identifer for object to which attribute is
    /// attached</param>
    /// <param name="attr_name">Name of attribute to open</param>
    /// <param name="aapl_id">Attribute access property list</param>
    /// <returns>Returns an attribute identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aopen", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t open
        (hid_t obj_id, string attr_name, hid_t aapl_id = H5P.DEFAULT);

    /// <summary>
    /// Reads an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Read
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute to read.</param>
    /// <param name="type_id"> Identifier of the attribute datatype
    /// (in memory).</param>
    /// <param name="buf">Buffer for data to be read.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aread"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t read
        (hid_t attr_id, hid_t type_id, Span<byte> buf);

    /// <summary>
    /// Reads an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Read
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute to read.</param>
    /// <param name="type_id"> Identifier of the attribute datatype
    /// (in memory).</param>
    /// <param name="buf">Buffer for data to be read.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aread"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static unsafe partial herr_t read
        (hid_t attr_id, hid_t type_id, IntPtr buf);

    /// <summary>
    /// Writes data to an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Write
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute to write.</param>
    /// <param name="mem_type_id">Identifier of the attribute datatype
    /// (in memory).</param>
    /// <param name="buf">Data to be written.</param>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Awrite"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t write
        (hid_t attr_id, hid_t mem_type_id, Span<byte> buf);
#endif
}
