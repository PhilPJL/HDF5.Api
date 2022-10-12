﻿using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5AAdapter.CreatePropertyList(listType);
    }

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public H5Type GetH5Type()
    {
        return H5AAdapter.GetType(this);
    }

    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5AAdapter.GetPropertyList(this, listType);
    }

    public string ReadString()
    {
        return H5AAdapter.ReadString(this);
    }

    public T Read<T>() where T : unmanaged
    {
        return H5AAdapter.Read<T>(this);
    }

    public DateTime ReadDateTime()
    {
        return H5AAdapter.ReadDateTime(this);
    }

    public void Write(string value)
    {
        H5AAdapter.Write(this,  value);
    }

    public void Write<T>(T value) where T : unmanaged
    {
        H5AAdapter.Write(this,  value);
    }

    public void Write(DateTime value)
    {
        H5AAdapter.Write(this, value);
    }
}