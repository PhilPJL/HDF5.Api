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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcommit2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t commit
        (hid_t loc_id, string name, hid_t dtype_id,
        hid_t lcpl_id = H5P.DEFAULT, hid_t tcpl_id = H5P.DEFAULT,
        hid_t tapl_id = H5P.DEFAULT);

    /// <summary>
    /// Determines whether a datatype is a named type or a transient type.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Committed
    /// </summary>
    /// <param name="dtype_id">Datatype identifier.</param>
    /// <returns>When successful, returns a positive value, for
    /// <code>TRUE</code>, if the datatype has been committed, or 0 (zero),
    /// for <code>FALSE</code>, if the datatype has not been committed.
    /// Otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcommitted"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial htri_t committed(hid_t dtype_id);

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
    /// Creates a new enumeration datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-EnumCreate
    /// </summary>
    /// <param name="dtype_id">Datatype identifier for the base datatype. 
    /// Must be an integer datatype.</param>
    /// <returns>Returns the datatype identifier for the new enumeration
    /// datatype if successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tenum_create"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t enum_create(hid_t dtype_id);

    /// <summary>
    /// Inserts a new enumeration datatype member.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-EnumInsert
    /// </summary>
    /// <param name="dtype_id">Datatype identifier for the enumeration
    /// datatype.</param>
    /// <param name="name">Name of the new member.</param>
    /// <param name="value">Pointer to the value of the new member.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tenum_insert", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t enum_insert
        (hid_t dtype_id, string name, IntPtr value);

    /// <summary>
    /// Returns the symbol name corresponding to a specified member of an
    /// enumeration datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-EnumNameOf
    /// </summary>
    /// <param name="dtype_id">Enumeration datatype identifier.</param>
    /// <param name="value">Value of the enumeration datatype.</param>
    /// <param name="name">Buffer for output of the symbol name.</param>
    /// <param name="size">The capacity of the buffer, in bytes
    /// (characters).</param>
    /// <returns>Returns a non-negative value if successful. Otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tenum_nameof"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t enum_nameof
        (hid_t dtype_id, IntPtr value, Span<byte> name, size_t size);

    /// <summary>
    /// Returns the value corresponding to a specified member of an
    /// enumeration datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-EnumValueOf
    /// </summary>
    /// <param name="dtype_id">Enumeration datatype identifier.</param>
    /// <param name="name">Symbol name of the enumeration datatype.</param>
    /// <param name="value">Buffer for output of the value of the
    /// enumeration datatype.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tenum_valueof", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t enum_valueof
        (hid_t dtype_id, string name, IntPtr value);

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
    /// Returns the native datatype of a specified datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetNativeType
    /// </summary>
    /// <param name="dtype_id">Datatype identifier for the dataset
    /// datatype.</param>
    /// <param name="direction">Direction of search.</param>
    /// <returns>Returns the native datatype identifier for the specified
    /// dataset datatype if successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_native_type"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_native_type
        (hid_t dtype_id, direction_t direction);

    /// <summary>
    /// Retrieves the number of elements in a compound or enumeration
    /// datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetNmembers
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns the number of elements if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_nmembers"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial int get_nmembers(hid_t dtype_id);

    /// <summary>
    /// Returns the size of a datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetSize
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns the size of the datatype in bytes if successful;
    /// otherwise 0.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_size"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial size_t get_size(hid_t dtype_id);

    /// <summary>
    /// Retrieves the type of padding used for a string datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-GetStrpad
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to query.</param>
    /// <returns>Returns a valid string storage mechanism if successful;
    /// otherwise <code>H5T.str_t.ERROR</code>.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tget_strpad"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial str_t get_strpad(hid_t dtype_id);

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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tinsert", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
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
    /// Locks a datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Lock
    /// </summary>
    /// <param name="dtype_id">Identifier of datatype to lock.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tlock"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t lock_datatype(hid_t dtype_id);

    /// <summary>
    /// Opens a committed (named) datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-Open2
    /// </summary>
    /// <param name="loc_id">A file or group identifier.</param>
    /// <param name="name">A datatype name, defined within the file or
    /// group identified by <paramref name="loc_id"/>.</param>
    /// <param name="tapl_id">Datatype access property list identifier.</param>
    /// <returns>Returns a committed datatype identifier if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Topen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(Utf8StringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t open
        (hid_t loc_id, string name, hid_t tapl_id = H5P.DEFAULT);

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
    /// Tags an opaque datatype.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5T.html#Datatype-SetTag
    /// </summary>
    /// <param name="dtype_id">Datatype identifier for the opaque datatype
    /// to be tagged.</param>
    /// <param name="tag">Descriptive ASCII string with which the opaque
    /// datatype is to be tagged.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks><paramref name="tag"/> is intended to provide a concise
    /// description; the maximum size is hard-coded in the HDF5 Library as
    /// 256 bytes </remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tset_tag", StringMarshalling = StringMarshalling.Utf8),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_tag(hid_t dtype_id, string tag);

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
