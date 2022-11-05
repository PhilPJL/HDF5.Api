using CommunityToolkit.Diagnostics;
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
        // TODO: allow bool as byte, long, bitfield

        // TODO: verify
        return H5AAdapter.ReadBool(this);
    }

    public override H5BooleanAttribute Write([DisallowNull] bool value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.WriteBool(this, value);

        return this;
    }
}
