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
        return H5AAdapter.GetType(this, static h => new H5DateTimeType(h));
    }

    public override DateTime Read()
    {
        // TODO: optionally write value.ToString("O")
        using var type = GetAttributeType();
        using var expectedType = H5DateTimeType.Create();

        long dateTime = H5AAdapter.ReadImpl<long>(this, type, expectedType);
        return DateTime.FromBinary(dateTime);
    }

    public override void Write([DisallowNull] DateTime value)
    {
        // TODO: optionally write value.ToString("O")
        using var type = H5TAdapter.ConvertDotNetPrimitiveToH5NativeType<long>();
        H5AAdapter.Write(this, type, value.ToBinary());
    }

    public static H5DateTimeAttribute Create(long handle)
    {
        return new H5DateTimeAttribute(handle);
    }
}
