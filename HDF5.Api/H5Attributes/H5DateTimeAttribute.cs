using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5DateTimeAttribute : H5Attribute<DateTime, H5DateTimeAttribute, H5DateTimeType>
{
    internal H5DateTimeAttribute(long handle) : base(handle)
    {
    }

    public override H5DateTimeType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5DateTimeType(h));
    }

    public override DateTime Read()
    {
        return H5AAdapter.ReadDateTime(this);
    }

    public override H5DateTimeAttribute Write([DisallowNull] DateTime value)
    {
        H5AAdapter.WriteDateTime(this, value);
        return this;
    }
}
