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
    private static partial long H5Acreate(long loc_id, string attr_name, long type_id, long space_id, long acpl_id, long aapl_id);

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
        H5PropertyList creationPropertyList = default, H5PropertyList accessPropertyList = default)
    {
        owner.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        var handle = H5A.create(owner, name, type, space, creationPropertyList, accessPropertyList);

        return new H5Attribute(handle);
    }

    #endregion

    #region Open

    /// <summary>
    /// Open an attribute
    /// </summary>
    /// <param name="objectId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static H5Attribute Open(long objectId, string name)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        Handle h = H5A.open(objectId, name);

        h.ThrowIfInvalidHandleValue("H5A.open");

        return new H5Attribute(h);
    }

    #endregion

    #region Delete

    public static void Delete(long objectId, string name)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5A.delete(objectId, name);

        err.ThrowIfError("H5A.delete");
    }

    #endregion

    #region Exists

    public static bool Exists(long objectId, string name)
    {
        objectId.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        int err = H5A.exists(objectId, name);

        err.ThrowIfError("H5A.exists");
        return err > 0;
    }

    #endregion

    #region List attribute names

    public static IEnumerable<string> ListAttributeNames(long handle)
    {
        handle.AssertIsHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        ulong idx = 0;

        var names = new List<string>();

        int err = H5A.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError("H5A.iterate");

        return names;

        int Callback(Handle id, IntPtr intPtrName, ref H5A.info_t info, IntPtr _)
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

    public static void Write(H5Attribute attribute, H5Type type, IntPtr buffer)
    {
        int err = H5A.write(attribute, type, buffer);

        err.ThrowIfError("H5A.write");
    }

    public static H5Space GetSpace(H5Attribute attribute)
    {
        return new H5Space(H5A.get_space(attribute));
    }

    public static long GetStorageSize(H5Attribute attributeId)
    {
        return (long)H5A.get_storage_size(attributeId);
    }
}
