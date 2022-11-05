using CommunityToolkit.HighPerformance;
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
        // TODO: save as byte Enum?
        using var type = GetAttributeType();
        using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();

        var value = H5AAdapter.ReadImpl<byte>(this, type, expectedType);
        return value != default;
    }

    public override H5BooleanAttribute Write([DisallowNull] bool value)
    {
        // TODO: save as byte Enum?
        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();
        H5AAdapter.Write(this, type, value.ToByte());

        return this;
    }
}
