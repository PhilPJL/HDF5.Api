using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal static partial class H5AttributeNativeMethods
{
    #region Close

    /// <summary>
    /// Closes the specified attribute.
    /// </summary>
    /// <param name="attr_id">Attribute to release access to.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial int H5Aclose(long attr_id);

    public static void Close(H5Attribute attribute)
    {
        H5Aclose(attribute).ThrowIfError(nameof(H5Aclose));
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates an attribute attached to a specified object.
    /// </summary>
    /// <param name="loc_id">Location or object identifier</param>
    /// <param name="attr_name">Attribute name</param>
    /// <param name="type_id">Attribute datatype identifier</param>
    /// <param name="space_id">Attribute dataspace identifier</param>
    /// <param name="acpl_id">Attribute creation property list identifier</param>
    /// <param name="aapl_id">Attribute access property list identifier</param>
    /// <returns>Returns an attribute identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Acreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Acreate2(long loc_id, string attr_name, long type_id, long space_id, long acpl_id, long aapl_id);

    /// <summary>
    /// Creates an attribute attached to a specified object.
    /// </summary>
    /// <param name="h5Object"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="space"></param>
    /// <param name="creationPropertyList"></param>
    /// <param name="accessPropertyList"></param>
    /// <returns></returns>
    public static H5Attribute Create<T>(H5Object<T> h5Object, string name, H5Type type, H5Space space,
        H5PropertyList? creationPropertyList = null, H5PropertyList? accessPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        var h = H5Acreate2(h5Object, name, type, space, creationPropertyList, accessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Acreate2));

        return new H5Attribute(h);
    }

    #endregion

    #region Open

    /// <summary>
    /// Opens an attribute for an object specified by object identifier
    /// and attribute name.
    /// </summary>
    /// <param name="obj_id">Identifer for object to which attribute is
    /// attached</param>
    /// <param name="attr_name">Name of attribute to open</param>
    /// <param name="aapl_id">Attribute access property list</param>
    /// <returns>Returns an attribute identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aopen", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Aopen(long obj_id, string attr_name, long aapl_id);

    /// <summary>
    /// Open an attribute
    /// </summary>
    /// <param name="h5Object"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static H5Attribute Open<T>(H5Object<T> h5Object, string name, H5PropertyList? attributeAccessPropertyList = default) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        long h = H5Aopen(h5Object, name, attributeAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5Aopen));

        return new H5Attribute(h);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Deletes an attribute from a specified location.
    /// </summary>
    /// <param name="loc_id">Identifier of the dataset, group, or named
    /// datatype to have the attribute deleted from.</param>
    /// <param name="name">Name of the attribute to delete.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Adelete", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Adelete(long loc_id, string name);

    public static void Delete<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5Adelete(h5Object, name);

        err.ThrowIfError(nameof(H5Adelete));
    }

    #endregion

    #region Exists

    /// <summary>
    /// Determines whether an attribute with a given name exists on an object.
    /// </summary>
    /// <param name="obj_id">Object identifier</param>
    /// <param name="attr_name">Attribute name</param>
    /// <returns>When successful, returns a positive value, for
    /// <code>TRUE</code>, or 0 (zero), for <code>FALSE</code>. Otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aexists", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Aexists(long obj_id, string attr_name);

    public static bool Exists<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5Aexists(h5Object, name);

        err.ThrowIfError(nameof(H5Aexists));
        return err > 0;
    }

    #endregion

    #region List attribute names
    /// <summary>
    /// Delegate for H5Aiterate2() callbacks
    /// </summary>
    /// <param name="location_id">The location identifier for the group or
    /// dataset being iterated over</param>
    /// <param name="attr_name">The name of the current object attribute.</param>
    /// <param name="ainfo">The attribute’s <code>info</code>struct</param>
    /// <param name="op_data">A pointer referencing operator data passed
    /// to <code>iterate</code></param>
    /// <returns>Valid return values from an operator and the resulting
    /// H5Aiterate2 and op behavior are as follows: Zero causes the iterator
    /// to continue, returning zero when all attributes have been processed.
    /// A positive value causes the iterator to immediately return that
    /// positive value, indicating short-circuit success. The iterator can
    /// be restarted at the next attribute, as indicated by the return
    /// value of <code>n</code>. A negative value causes the iterator to
    /// immediately return that value, indicating failure. The iterator can
    /// be restarted at the next attribute, as indicated by the return value
    /// of <code>n</code>.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int operator_t(long location_id, IntPtr attr_name, ref AttributeInfo ainfo, IntPtr op_data);

    /// <summary>
    /// Calls user-defined function for each attribute on an object.
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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aiterate2")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Aiterate2
        (long obj_id, GroupOrAttributeIndex idx_type, H5.iter_order_t order, ref ulong n, operator_t op, IntPtr op_data);

    public static IEnumerable<string> ListAttributeNames<T>(H5Object<T> h5Object, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = H5Aiterate2(h5Object, GroupOrAttributeIndex.Name, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError(nameof(H5Aiterate2));

        return names;

        int Callback(long id, IntPtr intPtrName, ref AttributeInfo info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);

            Guard.IsNotNull(name);

            info = GetInfoByName(h5Object, ".", name, linkAccessPropertyList);

            names.Add(name);
            return 0;
        }
    }

    #endregion

    #region GetInfoByName
    /// <summary>
    /// Retrieves attribute information, by attribute name.
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
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_info_by_name", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Aget_info_by_name(long loc_id, string obj_name, string attr_name, ref AttributeInfo ainfo, long lapl_id);

    // TODO: make public?
    private static AttributeInfo GetInfoByName<T>(H5Object<T> h5Object, string objectName, string attributeName, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        AttributeInfo info = default;
        int err = H5Aget_info_by_name(h5Object, objectName, attributeName, ref info, linkAccessPropertyList);
        err.ThrowIfError(nameof(H5Aget_info_by_name));
        return info;
    }

    /// <summary>
    /// Information struct for attribute (for H5Aget_info/H5Aget_info_by_idx).  Equivalent to H5A.info_t.
    /// </summary>
    private struct AttributeInfo
    {
        /// <summary>
        /// Indicate if creation order is valid
        /// </summary>
        public uint corder_valid;
        /// <summary>
        /// Creation order
        /// </summary>
        public uint corder;
        /// <summary>
        /// Character set of attribute name
        /// </summary>
        public CharacterSet cset;
        /// <summary>
        /// Size of raw data
        /// </summary>
        public ulong data_size;
    };

    private enum CharacterSet : int
    {
        /// <summary>
        /// error [value = -1].
        /// </summary>
        Error = -1,
        /// <summary>
        /// US ASCII [value = 0].
        /// </summary>
        Ascii = 0,
        /// <summary>
        /// UTF-8 Unicode encoding [value = 1].
        /// </summary>
        Utf8 = 1,
    }
    #endregion

    #region Write

    /// <summary>
    /// Writes data to an attribute.
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute to write.</param>
    /// <param name="mem_type_id">Identifier of the attribute datatype
    /// (in memory).</param>
    /// <param name="buf">Data to be written.</param>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Awrite")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Awrite(long attr_id, long mem_type_id, IntPtr buf);

    public static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = H5Awrite(attribute, type, buffer);

        err.ThrowIfError(nameof(H5Awrite));
    }

    #endregion

    #region GetSpace

    /// <summary>
    /// Gets a copy of the dataspace for an attribute.
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute.</param>
    /// <returns>Returns attribute dataspace identifier if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_space")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Aget_space(long attr_id);

    public static H5Space GetSpace(H5Attribute attribute)
    {
        var space = H5Aget_space(attribute);

        space.ThrowIfError(nameof(H5Aget_space));

        return new H5Space(space);
    }

    #endregion

    #region GetStorageSize

    /// <summary>
    /// Returns the amount of storage required for an attribute.
    /// </summary>
    /// <param name="attr_id">Identifier of the attribute to query.</param>
    /// <returns>Returns the amount of storage size allocated for the
    /// attribute; otherwise returns 0 (zero).</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_storage_size")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial ulong H5Aget_storage_size(long attr_id);

    public static long GetStorageSize(H5Attribute attribute)
    {
        return (long)H5Aget_storage_size(attribute);
    }

    #endregion

    #region GetType

    /// <summary>
    /// Returns the type of an attribute.
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute.</param>
    /// <returns>Returns a datatype identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_type")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial long H5Aget_type(long attr_id);

    public static H5Type GetType(H5Attribute attribute)
    {
        long typeHandle = H5Aget_type(attribute);
        typeHandle.ThrowIfInvalidHandleValue(nameof(H5Aget_type));
        return new H5Type(typeHandle);
    }

    #endregion

    #region Read

    /// <summary>
    /// Reads an attribute.
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute to read.</param>
    /// <param name="type_id"> Identifier of the attribute datatype
    /// (in memory).</param>
    /// <param name="buf">Buffer for data to be read.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aread")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Aread(long attr_id, long type_id, Span<byte> buf);

    public static void Read<T>(H5Attribute attribute, H5Type type, Span<T> buffer) where T : unmanaged
    {
        int err = H5Aread(attribute, type, MemoryMarshal.AsBytes(buffer));

        err.ThrowIfError(nameof(H5Aread));
    }

    #endregion
}

