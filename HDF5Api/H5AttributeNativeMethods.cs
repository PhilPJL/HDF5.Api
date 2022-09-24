using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace HDF5Api;

internal static partial class H5AttributeNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial int H5Aclose(long handle);

    public static void Close(H5Attribute attribute)
    {
        H5Aclose(attribute).ThrowIfError("H5Aclose");
    }

    #endregion

    #region Create

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
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Acreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Acreate2(long loc_id, string attr_name, long type_id, long space_id, long acpl_id, long aapl_id);

    /// <summary>
    /// Creates an attribute attached to a specified object.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="space"></param>
    /// <param name="creationPropertyList"></param>
    /// <param name="accessPropertyList"></param>
    /// <returns></returns>
    public static H5Attribute Create(long owner, string name, H5Type type, H5Space space,
        H5PropertyList? creationPropertyList = null, H5PropertyList? accessPropertyList = null)
    {
        owner.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        var handle = H5Acreate2(owner, name, type, space, creationPropertyList, accessPropertyList);

        return new H5Attribute(handle);
    }

    #endregion

    #region Open

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
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aopen", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Aopen(long obj_id, string attr_name, long aapl_id);

    /// <summary>
    /// Open an attribute
    /// </summary>
    /// <param name="objectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static H5Attribute Open(long objectId, string name, H5PropertyList? attributeAccessPropertyList = default)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        long h = H5Aopen(objectId, name, attributeAccessPropertyList);

        h.ThrowIfInvalidHandleValue("H5A.open");

        return new H5Attribute(h);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Deletes an attribute from a specified location.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Delete
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

    public static void Delete(long objectId, string name)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5Adelete(objectId, name);

        err.ThrowIfError("H5A.delete");
    }

    #endregion

    #region Exists

    /// <summary>
    /// Determines whether an attribute with a given name exists on an object.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Exists
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

    public static bool Exists(long objectId, string name)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5Aexists(objectId, name);

        err.ThrowIfError("H5A.exists");
        return err > 0;
    }

    #endregion

    #region List attribute names
    /// <summary>
    /// Delegate for H5Aiterate2() callbacks
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Iterate2
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
    private delegate int operator_t(long location_id, IntPtr attr_name, ref long ainfo, IntPtr op_data);

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
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aiterate2")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Aiterate
        (long obj_id, H5.index_t idx_type, H5.iter_order_t order, ref long n, operator_t op, IntPtr op_data);

    public static IEnumerable<string> ListAttributeNames(long handle)
    {
        handle.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = H5A.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError("H5A.iterate");

        return names;

        int Callback(long id, IntPtr intPtrName, ref H5A.info_t info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);

            Guard.IsNotNull(name);

            int err1 = H5A.get_info_by_name(handle, ".", name, ref info);
            err1.ThrowIfError("H5A.get_info_by_name");

            Debug.WriteLine($"{name}: {info.data_size}");

            names.Add(name);
            return 0;
        }
    }

    #endregion

    #region Write

    /// <summary>
    /// Writes data to an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-Write
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

        err.ThrowIfError("H5A.write");
    }

    #endregion

    #region GetSpace

    /// <summary>
    /// Gets a copy of the dataspace for an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetSpace
    /// </summary>
    /// <param name="attr_id">Identifier of an attribute.</param>
    /// <returns>Returns attribute dataspace identifier if successful;
    /// otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_space")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial long H5Aget_space(long attr_id);

    public static H5Space GetSpace(H5Attribute attribute)
    {
        return new H5Space(H5Aget_space(attribute));
    }

    #endregion

    #region GetStorageSize

    /// <summary>
    /// Returns the amount of storage required for an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetStorageSize
    /// </summary>
    /// <param name="attr_id">Identifier of the attribute to query.</param>
    /// <returns>Returns the amount of storage size allocated for the
    /// attribute; otherwise returns 0 (zero).</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Aget_storage_size")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial ulong H5Aget_storage_size(long attr_id);
    
    public static long GetStorageSize(H5Attribute attributeId)
    {
        return (long)H5Aget_storage_size(attributeId);
    }

    #endregion

    #region GetType

    /// <summary>
    /// Returns the type of an attribute.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5A.html#Annot-GetType
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
        typeHandle.ThrowIfInvalidHandleValue("H5A.get_type");
        return new H5Type(typeHandle);
    }

    #endregion
}
