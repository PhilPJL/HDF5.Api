using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HDF5Api;

/// <summary>
///     Intermediate base class for H5File and H5Group.
/// </summary>
/// <remarks>
///     Implements operations that can be carried out equally on files and groups.
/// </remarks>
/// <typeparam name="THandle"></typeparam>
public abstract class H5Location<THandle> : H5Object<THandle>, IH5Location where THandle : H5LocationHandle
{
    protected H5Location(THandle handle) : base(handle) { }

    /// <summary>
    ///     Create an Attribute for this location
    /// </summary>
    public H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId,
        H5PropertyListHandle propertyListId)
    {
        if (AttributeExists(name))
        {
            throw new Hdf5Exception($"Attribute {name} already exists");
        }

        return H5Attribute.Create(Handle, name, typeId, spaceId, propertyListId);
    }

    /// <summary>
    ///     Open an existing Attribute for this location
    /// </summary>
    public H5Attribute OpenAttribute(string name)
    {
        return H5Attribute.Open(Handle, name);
    }

    public void DeleteAttribute(string name)
    {
        H5Attribute.Delete(Handle, name);
    }

    public bool AttributeExists(string name)
    {
        return H5Attribute.Exists(Handle, name);
    }

    public T ReadAttribute<T>(string name) where T : unmanaged
    {
        return H5ObjectWithAttributeExtensions.ReadAttribute<T>(this, name);
    }

    public string ReadStringAttribute(string name)
    {
        return H5ObjectWithAttributeExtensions.ReadStringAttribute(this, name);
    }

    public DateTime ReadDateTimeAttribute(string name)
    {
        return H5ObjectWithAttributeExtensions.ReadDateTimeAttribute(this, name);
    }

    public IEnumerable<string> ListAttributeNames()
    {
        return H5Attribute.ListAttributeNames(Handle);
    }

    /// <summary>
    ///     Create a Group in this location
    /// </summary>
    public H5Group CreateGroup(string name)
    {
        return H5Group.Create(Handle, name);
    }

    /// <summary>
    ///     Open an existing group in this location
    /// </summary>
    public H5Group OpenGroup(string name)
    {
        return H5Group.Open(Handle, name);
    }

    public bool GroupExists(string name)
    {
        return H5Group.Exists(Handle, name);
    }

    public bool GroupPathExists(string path)
    {
        return H5Group.PathExists(Handle, path);
    }

    public void DeleteGroup(string path)
    {
        H5Group.Delete(Handle, path);
    }

    /// <summary>
    ///     Create a DataSet in this location
    /// </summary>
    public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId,
        H5PropertyListHandle propertyListId)
    {
        return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
    }

    /// <summary>
    ///     Open an existing DataSet in this location
    /// </summary>
    public H5DataSet OpenDataSet(string name)
    {
        return H5DataSet.Open(this, name);
    }

    public bool DataSetExists(string name)
    {
        return H5DataSet.Exists(this, name);
    }

    /// <summary>
    ///     Enumerate the names of child objects (groups, data-sets) in this location
    /// </summary>
    public IEnumerable<(string name, bool isGroup)> GetChildNames()
    {
        return GetChildNames(Handle);
    }

    private static IEnumerable<(string name, bool isGroup)> GetChildNames(H5LocationHandle handle)
    {
        handle.ThrowIfNotValid();

        ulong idx = 0;

        var names = new List<(string name, bool isGroup)>();

        int err = H5L.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError("H5L.iterate");

        return names;

        int Callback(Handle groupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr _)
        {
            string name = Marshal.PtrToStringAnsi(intPtrName);

            H5O.info_t oinfo = default;
            int err1 = H5O.get_info_by_name(groupId, name, ref oinfo);
            err1.ThrowIfError("H5O.get_info_by_name");

            names.Add((name, oinfo.type == H5O.type_t.GROUP));
            return 0;
        }
    }
}
