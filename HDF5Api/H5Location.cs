﻿using System;
using System.Collections.Generic;

namespace HDF5Api;

/// <summary>
///     Intermediate base class for H5File and H5Group.
/// </summary>
/// <remarks>
///     Implements operations that can be carried out equally on files and groups.
/// </remarks>
public abstract class H5Location<T> : H5Object<T>, IH5Location where T : H5Object<T>
{
    internal H5Location(long handle, Action<T>? closeHandle) : base(handle, closeHandle)
    {
    }

    /// <summary>
    ///     Create an Attribute for this location
    /// </summary>
    public H5Attribute CreateAttribute(string name, H5Type type, H5Space space, H5PropertyList propertyListId)
    {
        if (AttributeExists(name))
        {
            throw new Hdf5Exception($"Attribute {name} already exists");
        }

        return H5AttributeNativeMethods.Create(this, name, type, space, propertyListId);
    }

    /// <summary>
    ///     Open an existing Attribute for this location
    /// </summary>
    public H5Attribute OpenAttribute(string name)
    {
        return H5AttributeNativeMethods.Open(this, name);
    }

    public void DeleteAttribute(string name)
    {
        H5AttributeNativeMethods.Delete(this, name);
    }

    public bool AttributeExists(string name)
    {
        return H5AttributeNativeMethods.Exists(this, name);
    }

    public TA ReadAttribute<TA>(string name) where TA : unmanaged
    {
        return H5ObjectWithAttributeExtensions.ReadAttribute<T, TA>(this, name);
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
        return H5AttributeNativeMethods.ListAttributeNames(this);
    }

    /// <summary>
    ///     Create a Group in this location
    /// </summary>
    public H5Group CreateGroup(string name)
    {
        return H5GroupNativeMethods.Create(this, name);
    }

    /// <summary>
    ///     Open an existing group in this location
    /// </summary>
    public H5Group OpenGroup(string name)
    {
        return H5GroupNativeMethods.Open(this, name);
    }

    public bool GroupExists(string name)
    {
        return H5GroupNativeMethods.Exists(this, name);
    }

    public bool GroupPathExists(string path)
    {
        return H5GroupNativeMethods.PathExists(this, path);
    }

    public void DeleteGroup(string path)
    {
        H5GroupNativeMethods.Delete(this, path);
    }

    /// <summary>
    ///     Create a DataSet in this location
    /// </summary>
    public H5DataSet CreateDataSet(string name, H5Type type, H5Space space, H5PropertyList dataSetCreationPropertyList)
    {
        return H5DataSetNativeMethods.Create(this, name, type, space, null, dataSetCreationPropertyList);
    }

    /// <summary>
    ///     Open an existing DataSet in this location
    /// </summary>
    public H5DataSet OpenDataSet(string name)
    {
        return H5DataSetNativeMethods.Open(this, name);
    }

    public bool DataSetExists(string name)
    {
        return H5DataSetNativeMethods.Exists(this, name);
    }

    /// <summary>
    ///     Enumerate the names of child objects (groups, data-sets) in this location
    /// </summary>
    public IEnumerable<(string name, bool isGroup)> GetChildNames()
    {
        return GetChildNames(this);
    }

    private static IEnumerable<(string name, bool isGroup)> GetChildNames(H5Location<T> handle)
    {
        ulong idx = 0;

        var names = new List<(string name, bool isGroup)>();

        int err = H5L.iterate(handle, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError("H5L.iterate");

        return names;

        int Callback(long groupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);

            H5O.info_t oinfo = default;
            int err1 = H5O.get_info_by_name(groupId, name, ref oinfo);
            err1.ThrowIfError("H5O.get_info_by_name");

            // TODO: nullability - converting to "(unknown)" isn't great

            names.Add((name ?? "(unknown)", oinfo.type == H5O.type_t.GROUP));
            return 0;
        }
    }
}
