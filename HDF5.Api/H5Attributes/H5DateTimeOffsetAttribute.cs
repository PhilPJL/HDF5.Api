using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DateTimeOffsetAttribute : H5Attribute<DateTimeOffset, H5DateTimeOffsetAttribute, H5DateTimeOffsetType>
{
    internal H5DateTimeOffsetAttribute(long handle) : base(handle)
    {
    }

    public override H5DateTimeOffsetType GetAttributeType()
    {
        return H5AAdapter.GetType(this, static h => new H5DateTimeOffsetType(h));
    }

    public override DateTimeOffset Read()
    {
        // TODO: optionally write value.ToString("O")
        using var type = GetAttributeType();
        using var expectedType = H5DateTimeOffsetType.Create();

        var value = H5AAdapter.ReadImpl<DateTimeOffsetProxy>(this, type, expectedType);

        return new DateTimeOffset(DateTime.FromBinary(value.DateTime), TimeSpan.FromMinutes(value.Offset));
    }

    public override void Write([DisallowNull] DateTimeOffset value)
    {
        var dt = new DateTimeOffsetProxy
        {
            DateTime = value.DateTime.ToBinary(),
            Offset = (short)value.Offset.TotalMinutes
        };

        // TODO: optionally write value.ToString("O")
        using var type = H5DateTimeOffsetType.Create();
        H5AAdapter.Write(this, type, dt);
    }

    public static H5DateTimeOffsetAttribute Create(long handle)
    {
        return new H5DateTimeOffsetAttribute(handle);
    }
}
