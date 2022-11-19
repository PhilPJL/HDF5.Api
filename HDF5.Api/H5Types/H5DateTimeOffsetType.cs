using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5DateTimeOffsetType : H5CompoundType<DateTimeOffset>
{
    internal H5DateTimeOffsetType(long handle) : base(handle)
    {
    }

    internal static H5DateTimeOffsetType Create()
    {
        var type = H5TAdapter.CreateCompoundType<DateTimeOffsetProxy, H5DateTimeOffsetType>(static h => new H5DateTimeOffsetType(h));

        try
        {
            type.Insert<DateTimeOffsetProxy, long>(nameof(DateTimeOffsetProxy.DateTime));
            type.Insert<DateTimeOffsetProxy, int>(nameof(DateTimeOffsetProxy.Offset));
            return type;
        }
        catch
        {
            type.Dispose();
            throw;
        }
    }
}
