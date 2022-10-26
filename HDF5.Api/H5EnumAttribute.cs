using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethodAdapters;
namespace HDF5.Api;

public class H5EnumAttribute<T> : H5Attribute<T>
    where T : unmanaged, Enum
{
    internal H5EnumAttribute(long handle) : base(handle)
    {
    }

#if NET7_0_OR_GREATER
    public override H5EnumType<T> 
#else
    public override H5Type
#endif
    GetH5Type()
    {
        return H5AAdapter.GetType(this, h => new H5EnumType<T>(h));
    }

    public override T Read()
    {
        throw new NotImplementedException();
    }

    public override void Write(T value)
    {
        Guard.IsNotNull(value);

        H5AAdapter.Write(this, value);
    }
}
