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
internal static partial class H5T
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Creates an array datatype object.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-ArrayCreate2
    /// </summary>
    /// <param name="base_type_id">Datatype identifier for the array base
    /// datatype.</param>
    /// <param name="rank">Rank of the array.</param>
    /// <param name="dims">Size of each array dimension.</param>
    /// <returns>Returns a valid datatype identifier if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tarray_create2"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t array_create
        (hid_t base_type_id, uint rank,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] hsize_t[] dims);

    /// <summary>
    /// Releases a datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Close
    /// </summary>
    /// <param name="type_id">Identifier of datatype to release.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t type_id);

    /// <summary>
    /// Copies an existing datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Copy
    /// </summary>
    /// <param name="type_id">Identifier of datatype to copy.</param>
    /// <returns>Returns a datatype identifier if successful; otherwise
    /// returns a negative value</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcopy"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t copy(hid_t type_id);

    /// <summary>
    /// Creates a new datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Create
    /// </summary>
    /// <param name="cls">Class of datatype to create.</param>
    /// <param name="size">Size, in bytes, of the datatype being created</param>
    /// <returns>Returns datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcreate"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create(class_t cls, size_t size);

    /// <summary>
    /// Returns the datatype class identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetClass
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns datatype class identifier if successful; otherwise
    /// <code>H5T_NO_CLASS</code>.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_class"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial class_t get_class(hid_t dtype_id);

    /// <summary>
    /// Adds a new member to a compound datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Insert
    /// </summary>
    /// <param name="dtype_id">Identifier of compound datatype to modify.</param>
    /// <param name="name">Name of the field to insert.</param>
    /// <param name="offset">Offset in memory structure of the field to
    /// insert.</param>
    /// <param name="field_id">Datatype identifier of the field to insert.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tinsert", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t insert
        (hid_t dtype_id, string name, size_t offset, hid_t field_id);


    /// <summary>
    /// Determines whether datatype is a variable-length string.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-IsVariableString
    /// </summary>
    /// <param name="dtype_id">Datatype identifier.</param>
    /// <returns>Returns <code>TRUE</code> or <code>FALSE</code> if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tis_variable_str"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t is_variable_str(hid_t dtype_id);

    /// <summary>
    /// Sets the total size for a datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-SetSize
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype for which the size is
    /// being changed</param>
    /// <param name="size">New datatype size in bytes or <code>VARIABLE</code></param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tset_size"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_size(hid_t dtype_id, size_t size);

    /// <summary>
    /// Creates a new variable-length array datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-VLCreate
    /// </summary>
    /// <param name="base_type_id">Base type of datatype to create.</param>
    /// <returns>Returns datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tvlen_create"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t vlen_create(hid_t base_type_id);
#endif
}
