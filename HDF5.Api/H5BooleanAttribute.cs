using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5BooleanAttribute : H5Attribute<bool>
{
    internal H5BooleanAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5Type<bool> GetH5Type()
#else
    public override H5Type GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5BooleanType(h));
    }

    public override bool Read()
    {
        return H5AAdapter.Read(this);
    }

    public override H5Attribute<bool> Write([DisallowNull] bool value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);

        return this;
    }
}
