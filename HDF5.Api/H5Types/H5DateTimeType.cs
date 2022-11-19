using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

internal class H5DateTimeType : H5Type<DateTime>
{
    internal H5DateTimeType(long handle) : base(handle)
    {
    }

    internal static H5DateTimeType Create()
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5Type<long, H5DateTimeType>(static h => new H5DateTimeType(h));
    }
}
