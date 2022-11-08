using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5BooleanAttribute : H5Attribute<bool, H5BooleanAttribute, H5BooleanType>
{
    internal H5BooleanAttribute(long handle) : base(handle)
    {
    }

    public override H5BooleanType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5BooleanType(h));
    }

    public override bool Read(bool verifyType = false)
    {
        using var type = GetAttributeType();
        using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();

        return H5AAdapter.ReadImpl<bool>(this, type, expectedType);
    }

    public override void Write([DisallowNull] bool value)
    {
        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();
        H5AAdapter.Write(this, type, value); 
    }

    public static H5BooleanAttribute Create(long handle)
    {
        return new H5BooleanAttribute(handle);
    }
}
