using CommunityToolkit.Diagnostics;
using HDF5.Api.H5Types;
using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Attributes;

public class H5StringAttribute : H5Attribute<string, H5StringAttribute, H5StringType>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
    }

    public override H5StringType GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5StringType(h));
    }

    public override string Read(bool verifyType = false)
    {
        return H5AAdapter.ReadString(this);
    }

    public override H5StringAttribute Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}

