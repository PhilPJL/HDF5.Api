using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5PrimitiveAttribute<T> : H5Attribute<T, H5PrimitiveAttribute<T>, H5PrimitiveType<T>> //where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
    }

    public override H5PrimitiveType<T> GetH5Type()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        return H5AAdapter.GetType(this, h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        return H5AAdapter.Read<T>(this);
    }

    public override H5PrimitiveAttribute<T> Write([DisallowNull] T value) //where T : unmanaged
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}
