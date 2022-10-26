using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5StringAttribute : H5Attribute<string>
{
    internal H5StringAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5StringType GetH5Type()
#else
    public override H5Type GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5StringType(h));
    }

    public override string Read()
    {
        return H5AAdapter.ReadString(this);
    }

    public override void Write([DisallowNull] string value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);
    }
}
