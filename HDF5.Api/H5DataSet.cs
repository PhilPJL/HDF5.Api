using CommunityToolkit.Diagnostics;
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
        [DisallowNull] string name, [DisallowNull] H5Type type, [DisallowNull] H5Space space,
        [AllowNull] H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        Guard.IsNotNull(type);
        Guard.IsNotNull(space);

        return H5AAdapter.Create(this, name, type, space, creationPropertyList);
    }

    public H5Attribute CreateStringAttribute(
        [DisallowNull] string name,
        int fixedStorageLength = 0, CharacterSet characterSet = CharacterSet.Ascii, StringPadding padding = StringPadding.NullTerminate,
        [AllowNull] H5PropertyList? creationPropertyList = null)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        return H5AAdapter.CreateStringAttribute(this, name, fixedStorageLength, characterSet, padding, creationPropertyList);
    }

    public static H5PropertyList CreatePropertyList(PropertyListType listType)
    {
        return H5DAdapter.CreatePropertyList(listType);
    }

    public void DeleteAttribute([DisallowNull] string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        H5AAdapter.Delete(this, name);
    }

    public H5PropertyList GetPropertyList(PropertyListType listType)
    {
        return H5DAdapter.GetPropertyList(this, listType);
    }

    public H5Type GetH5Type()
    {
        return H5DAdapter.GetType(this);
    }

    public string GetName => H5IAdapter.GetName(this);

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

        H5DAdapter.Write(this, type, memorySpace, fileSpace, buffer);
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