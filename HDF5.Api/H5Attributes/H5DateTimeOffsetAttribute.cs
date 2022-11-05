using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DateTimeOffsetAttribute : H5Attribute<DateTimeOffset, H5DateTimeOffsetAttribute, H5DateTimeOffsetType>
{
    internal H5DateTimeOffsetAttribute(long handle) : base(handle)
    {
    }

    public override H5DateTimeOffsetType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5DateTimeOffsetType(h));
    }

    public override DateTimeOffset Read(bool verifyType = false)
    {
        // TODO: optionally write value.ToString("O")
        using var type = GetH5Type();
        using var expectedType = H5TAdapter.CreateDateTimeOffsetType();

        // TODO: sort out the type/expectedType/cls stuff
        var value = H5AAdapter.ReadImpl<DateTimeOffsetProxy>(this, type, expectedType);

        return new DateTimeOffset(DateTime.FromBinary(value.DateTime), TimeSpan.FromMinutes(value.Offset));
    }

    public override H5DateTimeOffsetAttribute Write([DisallowNull] DateTimeOffset value)
    {
        var dt = new DateTimeOffsetProxy
        {
            DateTime = value.DateTime.ToBinary(),
            Offset = (short)value.Offset.TotalMinutes
        };

        // TODO: optionally write value.ToString("O")
        using var type = H5TAdapter.CreateDateTimeOffsetType();
        H5AAdapter.Write(this, type, dt);

        return this;
    }
}
