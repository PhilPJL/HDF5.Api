using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

#if NET7_0_OR_GREATER

public class H5DateOnlyType : H5Type<DateOnly>
{
    internal H5DateOnlyType(long handle) : base(handle)
    {
    }

    internal static H5DateOnlyType Create()
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5Type<int, H5DateOnlyType>(h => new H5DateOnlyType(h));
    }
}

#endif
