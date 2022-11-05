using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public abstract class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public string Name => H5AAdapter.GetName(this);

    internal H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    internal int StorageSize => H5AAdapter.GetStorageSize(this);
}
