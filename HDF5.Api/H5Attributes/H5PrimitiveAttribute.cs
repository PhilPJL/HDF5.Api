using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5PrimitiveAttribute<T> : H5Attribute<T, H5PrimitiveAttribute<T>, H5PrimitiveType<T>> //where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
    }

    public override H5PrimitiveType<T> GetAttributeType()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        return H5AAdapter.GetType(this, h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        H5ThrowHelpers.ThrowIfManaged<T>();

        using var type = GetAttributeType();
        using var expectedType = H5PrimitiveType<T>.Create();
        return H5AAdapter.ReadImpl<T>(this, type, expectedType);
    }

    public override void Write([DisallowNull] T value) //where T : unmanaged
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
        Guard.IsNotNull(value);

        using var type = GetAttributeType();
        H5AAdapter.Write(this, type, value);
    }

    public static H5PrimitiveAttribute<T> Create(long handle)
    {
        return new H5PrimitiveAttribute<T>(handle);
    }
}
