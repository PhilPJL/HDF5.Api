using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DateTimeAttribute : H5Attribute<DateTime, H5DateTimeAttribute, H5DateTimeType>
{
    internal H5DateTimeAttribute(long handle) : base(handle)
    {
    }

    public override H5DateTimeType GetAttributeType()
    {
        return H5AAdapter.GetType(this, h => new H5DateTimeType(h));
    }

    public override DateTime Read(bool verifyType = false)
    {
        // TODO: optionally write value.ToString("O")
        using var type = GetAttributeType();

        // TODO: sort out the type/expectedType/cls stuff
        long dateTime = H5AAdapter.ReadImpl<long>(this, type, type);
        return DateTime.FromBinary(dateTime);
    }

    public override H5DateTimeAttribute Write([DisallowNull] DateTime value)
    {
        // TODO: optionally write value.ToString("O")
        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.ToBinary());

        return this;
    }
}
