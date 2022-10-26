using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5PrimitiveAttribute<T> : H5Attribute<T> where T : unmanaged
{
    internal H5PrimitiveAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5PrimitiveType<T> GetH5Type()
#else
    public override H5Type GetH5Type()
#endif
    {
        return H5AAdapter.GetType(this, h => new H5PrimitiveType<T>(h));
    }

    public override T Read()
    {
        return H5AAdapter.Read<T>(this);
    }

    public override void Write([DisallowNull] T value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);
    }
}
