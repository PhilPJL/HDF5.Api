using CommunityToolkit.HighPerformance;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5BooleanAttribute : H5Attribute<bool, H5BooleanAttribute, H5BooleanType>
{
    internal H5BooleanAttribute(long handle) : base(handle)
    {
    }

    public override H5BooleanType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5BooleanType(h));
    }

    public override bool Read(bool verifyType = false)
    {
        // TODO: save as byte, bitmask or long?

        using var type = GetH5Type();
        using var expectedType = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();

        byte value = H5AAdapter.ReadImpl<byte>(this, type, expectedType);
        return value != default;
    }

    public override H5BooleanAttribute Write([DisallowNull] bool value)
    {
        // TODO: save as byte, bitmask or long?

        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<byte>();
        H5AAdapter.Write(this, type, value.ToByte());

        return this;
    }
}
