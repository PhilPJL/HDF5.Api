using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5BooleanType : H5Type<bool>
{
    internal H5BooleanType(long handle) : base(handle)
    {
    }

    internal static H5BooleanType Create()
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5Type<bool, H5BooleanType>(h => new H5BooleanType(h));
    }
}
