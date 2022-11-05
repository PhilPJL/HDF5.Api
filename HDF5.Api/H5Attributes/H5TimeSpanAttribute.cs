using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5TimeSpanAttribute : H5Attribute<TimeSpan, H5TimeSpanAttribute, H5TimeSpanType>
{
    internal H5TimeSpanAttribute(long handle) : base(handle)
    {
    }

    public override H5TimeSpanType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5TimeSpanType(h));
    }

    public override TimeSpan Read(bool verifyType = false)
    {
        return H5AAdapter.ReadTimeSpan(this);
    }

    public override H5TimeSpanAttribute Write([DisallowNull] TimeSpan value)
    {
        H5AAdapter.WriteTimeSpan(this, value);
        return this;
    }
}

