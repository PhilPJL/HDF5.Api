using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

/// <summary>
///     <para>.NET wrapper for H5A (Attribute) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_a.html"/>
/// </summary>
public abstract class H5Attribute : H5Object<H5Attribute>
{
    internal H5Attribute(long handle) : base(handle, HandleType.Attribute, H5AAdapter.Close)
    {
    }

    public H5Space GetSpace()
    {
        return H5AAdapter.GetSpace(this);
    }

    public abstract H5Type GetH5Type();

    internal H5AttributeCreationPropertyList GetCreationPropertyList()
    {
        return H5AAdapter.GetCreationPropertyList(this);
    }

    public int StorageSize => H5AAdapter.GetStorageSize(this);

    internal static H5AttributeCreationPropertyList CreateCreationPropertyList(CharacterSet encoding = CharacterSet.Utf8)
    {
        return H5AAdapter.CreateCreationPropertyList(encoding);
    }

    public string Name => H5AAdapter.GetName(this);
}

public abstract class H5Attribute<T> : H5Attribute
{
    internal H5Attribute(long handle) : base(handle)
    {
    }

    public abstract void Write([DisallowNull] T value);

#if NET7_0_OR_GREATER
    public override H5Type<T>
#else
    public override H5Type
#endif       
    GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5Type<T>(h));
    }

    public abstract T Read();

    [DisallowNull]
    public T Value
    {
        get => Read();
        set => Write(value);
    }
}
