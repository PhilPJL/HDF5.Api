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
    /// Commits a transient datatype, linking it into the file and creating
    /// a new named datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Commit2
    /// </summary>
    /// <param name="loc_id">Location identifier</param>
    /// <param name="name">Name given to committed datatype</param>
    /// <param name="dtype_id">Identifier of datatype to be committed and,
    /// upon function’s return, identifier for the committed datatype</param>
    /// <param name="lcpl_id">Link creation property list</param>
    /// <param name="tcpl_id">Datatype creation property list</param>
    /// <param name="tapl_id">Datatype access property list</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcommit2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t commit
        (hid_t loc_id, string name, hid_t dtype_id,
        hid_t lcpl_id = H5P.DEFAULT, hid_t tcpl_id = H5P.DEFAULT,
        hid_t tapl_id = H5P.DEFAULT);

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
    /// Determines whether two datatype identifiers refer to the same
    /// datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Equal
    /// </summary>
    /// <param name="type_id1">Identifier of datatype to compare.</param>
    /// <param name="type_id2">Identifier of datatype to compare.</param>
    /// <returns>When successful, returns a positive value, for
    /// <code>TRUE</code>, if the datatype has been committed, or 0 (zero),
    /// for <code>FALSE</code>, if the datatype has not been committed.
    /// Otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tequal"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t equal(hid_t type_id1, hid_t type_id2);

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
    /// Retrieves the character set type of a string datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetCset
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns a valid character set type if successful;
    /// otherwise <code>H5T.cset_t.CSET_ERROR</code>.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_cset"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial cset_t get_cset(hid_t dtype_id);

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
    /// Sets character set to be used in a string or character datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-SetCset
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to modify.</param>
    /// <param name="cset">Character set type.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tset_cset"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_cset(hid_t dtype_id, cset_t cset);

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
    /// Defines the type of padding used for character strings.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-SetStrpad
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to modify.</param>
    /// <param name="strpad">String padding type.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tset_strpad"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_strpad(hid_t dtype_id, str_t strpad);

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
