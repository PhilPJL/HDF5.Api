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
        return H5AAdapter.ReadDateTimeOffset(this);
    }

    public override H5DateTimeOffsetAttribute Write([DisallowNull] DateTimeOffset value)
    {
        H5AAdapter.WriteDateTimeOffset(this, value);
        return this;
    }
}
