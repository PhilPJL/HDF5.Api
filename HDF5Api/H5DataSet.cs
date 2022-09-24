using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5D (Data-set) API.
/// </summary>
public class H5DataSet : H5Object<H5DataSet>, IH5ObjectWithAttributes
{
    internal H5DataSet(long handle) : base(handle, H5DataSetNativeMethods.Close)
    {
    }

    public void Write(H5Type typeId, H5Space memorySpace, H5Space fileSpace, IntPtr buffer)
    {
        H5DataSetNativeMethods.Write(this, typeId, memorySpace, fileSpace, buffer);
    }

    public H5Space GetSpace()
    {
        return H5DataSetNativeMethods.GetSpace(this);
    }

    public void SetExtent(ulong[] dims)
    {
        H5DataSetNativeMethods.SetExtent(this, dims);
    }

    public H5Type GetH5Type()
    {
        return H5TypeNativeMethods.GetType(this);
    }

    /// <summary>
    ///     Open an existing Attribute for this dataset
    /// </summary>
    public H5Attribute OpenAttribute(string name)
    {
        return H5AttributeNativeMethods.Open(Handle, name);
    }

    public H5Attribute CreateAttribute(string name, H5Type typeId, H5Space space, H5PropertyList? creationPropertyList = null)
    {
        return H5AttributeNativeMethods.Create(Handle, name, typeId, space, creationPropertyList);
    }

    public void DeleteAttribute(string name)
    {
        H5AttributeNativeMethods.Delete(Handle, name);
    }

    public bool AttributeExists(string name)
    {
        return H5AttributeNativeMethods.Exists(Handle, name);
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
        return H5AttributeNativeMethods.ListAttributeNames(Handle);
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// <returns></returns>
    public H5PropertyList GetCreationPropertyList()
    {
        return H5PropertyListNativeMethods.GetCreationPropertyList(this);
    }

    public IEnumerable<T> Read<T>() where T : unmanaged
    {
        using var space = GetSpace();
        using var type = GetH5Type();
        long count = space.GetSimpleExtentNPoints();

        var cls = type.GetClass();
        if (cls != H5T.class_t.COMPOUND)
        {
            throw new Hdf5Exception($"DataSet is of class {cls} when expecting {H5T.class_t.COMPOUND}.");
        }

        long size = H5DataSetNativeMethods.GetStorageSize(this);

        if (size != count * Marshal.SizeOf<T>())
        {
            throw new Hdf5Exception(
                $"Attribute storage size is {size}, which does not match the expected size for {count} items of type {typeof(T).Name} of {count * Marshal.SizeOf<T>()}.");
        }

        unsafe
        {
            var result = new T[count];
            fixed (T* ptr = result)
            {
                int err = H5D.read(Handle, type, space, space, 0, new IntPtr(ptr));
                err.ThrowIfError("H5A.read");
                return result;
            }
        }
    }
}