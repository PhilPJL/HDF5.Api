using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
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

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public H5Type GetH5Type()
    {
        return H5AAdapter.GetType(this);
    }

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    public int StorageSize => H5AAdapter.GetStorageSize(this);

    public string ReadString()
    {
        return H5AAdapter.ReadString(this);
    }

    public T Read<T>() where T : unmanaged
    {
        return H5AAdapter.Read<T>(this);
    }

    public T ReadEnum<T>() where T : unmanaged, Enum
    {
        return H5AAdapter.ReadEnum<T>(this);
    }

    public DateTime ReadDateTime()
    {
        return H5AAdapter.ReadDateTime(this);
    }

    public H5Attribute Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);
        H5AAdapter.Write(this, value);
        return this;
    }

    public H5Attribute Write<T>(T value) where T : unmanaged
    {
        H5AAdapter.Write(this, value);
        return this;
    }

    public H5Attribute Write(DateTime value)
    {
        H5AAdapter.Write(this, value);
        return this;
    }

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public string Name => H5AAdapter.GetName(this);
}
