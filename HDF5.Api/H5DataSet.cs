﻿using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5D (DataSet) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_d.html"/>
/// </summary>
public class H5DataSet : H5Object<H5DataSet>, IH5ObjectWithAttributes
{
    internal H5DataSet(long handle) : base(handle, HandleType.DataSet, H5DAdapter.Close)
    {
    }

    public bool AttributeExists([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Exists(this, name);
    }

    public IEnumerable<string> AttributeNames => H5AAdapter.GetAttributeNames(this);

    public H5Attribute CreateAttribute(
        [DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] H5Space space)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5AAdapter.Create(this, name, type, space);
    }

    public H5Attribute CreateStringAttribute(
        [DisallowNull] string name,
        int fixedStorageLength = 0, CharacterSet characterSet = CharacterSet.Ascii, StringPadding padding = StringPadding.NullTerminate)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding);
    }

    public void DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        H5AAdapter.Delete(this, name);
    }

    internal static H5DataSetCreationPropertyList CreateCreationPropertyList()
    {
        return H5DAdapter.CreateCreationPropertyList();
    }

    internal static H5DataSetAccessPropertyList CreateAccessPropertyList()
    {
        return H5DAdapter.CreateAccessPropertyList();
    }

    internal H5DataSetCreationPropertyList GetCreationPropertyList()
    {
        return H5DAdapter.GetCreationPropertyList(this);
    }

    internal H5DataSetAccessPropertyList GetAccessPropertyList()
    {
        return H5DAdapter.GetAccessPropertyList(this);
    }

    public H5Type GetH5Type()
    {
        return H5DAdapter.GetType(this);
    }

    public string Name => H5IAdapter.GetName(this);

    public H5Space GetSpace()
    {
        return H5DAdapter.GetSpace(this);
    }

    public int NumberOfAttributes => (int)H5OAdapter.GetInfo(this).num_attrs;

    /// <summary>
    ///     Open an existing Attribute for this dataset
    /// </summary>
    public H5Attribute OpenAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.Open(this, name);
    }

    public T ReadAttribute<T>([DisallowNull] string name) where T : unmanaged
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.Read<T>(attribute);
    }

    public string ReadStringAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadString(attribute);
    }

    public DateTime ReadDateTimeAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var attribute = H5AAdapter.Open(this, name);
        return H5AAdapter.ReadDateTime(attribute);
    }

    public IEnumerable<T> Read<T>() where T : unmanaged
    {
        return H5DAdapter.Read<T>(this);
    }

    public void SetExtent([DisallowNull] params long[] dims)
    {
        Guard.IsNotNull(dims);

        H5DAdapter.SetExtent(this, dims);
    }

    // TODO: get rid of IntPtr
    public void Write([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, [DisallowNull] IntPtr buffer)
    {
        Guard.IsNotNull(type);
        Guard.IsNotNull(memorySpace);
        Guard.IsNotNull(fileSpace);
        Guard.IsNotNull(buffer);

        H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer, null);
    }

    /*    public void Write([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, [DisallowNull] T[] buffer) where T : unmanaged
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(memorySpace);
            Guard.IsNotNull(fileSpace);
            Guard.IsNotNull(buffer);

            H5DAdapter.Write<T>(this, type, memorySpace, fileSpace, buffer);
        }
    */
    /*    public void Write<T>([DisallowNull] H5Type type, [DisallowNull] H5Space memorySpace, [DisallowNull] H5Space fileSpace, Span<T> buffer) where T : unmanaged
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(memorySpace);
            Guard.IsNotNull(fileSpace);

            H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer);
        }*/
}