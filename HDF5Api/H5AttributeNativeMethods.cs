using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
/// H5 attribute native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
internal static partial class H5AttributeNativeMethods
{
    public static void Close(H5Attribute attribute)
    {
        int err = H5A.close(attribute);

        err.ThrowIfError(nameof(H5A.close));
    }


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

        var h = H5A.create(h5Object, name, type, space, creationPropertyList, accessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(H5A.create));

        return new H5Attribute(h);
    }


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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Aopen(hid_t obj_id, string attr_name, hid_t aapl_id);

    /// <summary>
    /// Open an attribute
    /// </summary>
    /// <param name="h5Object"></param>
    /// <param name="name"></param>
    /// <param name="attributeAccessPropertyList"></param>
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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Adelete(hid_t loc_id, string name);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial htri_t H5Aexists(hid_t obj_id, string attr_name);

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
    private delegate herr_t operator_t(hid_t location_id, IntPtr attr_name, ref AttributeInfo ainfo, IntPtr op_data);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Aiterate2
        (hid_t obj_id, GroupOrAttributeIndex idx_type, CommonIterationOrders order, ref hsize_t n, operator_t op, IntPtr op_data);

    public static IEnumerable<string> ListAttributeNames<T>(H5Object<T> h5Object, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = H5Aiterate2(h5Object, GroupOrAttributeIndex.Name, CommonIterationOrders.Increasing, ref idx, Callback, IntPtr.Zero);
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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Aget_info_by_name(hid_t loc_id, string obj_name, string attr_name, ref AttributeInfo ainfo, hid_t lapl_id);

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
    }

    private enum CharacterSet
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
        Utf8 = 1
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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Awrite(hid_t attr_id, hid_t mem_type_id, IntPtr buf);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Aget_space(hid_t attr_id);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hsize_t H5Aget_storage_size(hid_t attr_id);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial hid_t H5Aget_type(hid_t attr_id);

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
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Aread(hid_t attr_id, hid_t type_id, Span<byte> buf);

    public static void Read<T>(H5Attribute attribute, H5Type type, Span<T> buffer) where T : unmanaged
    {
        int err = H5Aread(attribute, type, MemoryMarshal.AsBytes(buffer));

        err.ThrowIfError(nameof(H5Aread));
    }

    #endregion
}

