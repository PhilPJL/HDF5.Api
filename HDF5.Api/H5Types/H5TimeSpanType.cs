using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;
internal class H5TimeSpanType : H5Type<TimeSpan> 
{
    internal H5TimeSpanType(long handle) : base(handle)
    {
    }

    internal static H5TimeSpanType Create()
    {
        return H5TAdapter.ConvertDotNetPrimitiveToH5Type<long, H5TimeSpanType>(static h => new H5TimeSpanType(h));
    }
}
