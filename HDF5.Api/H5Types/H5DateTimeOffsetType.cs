using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

internal class H5DateTimeOffsetType : H5CompoundType<DateTimeOffset>
{
    internal H5DateTimeOffsetType(long handle) : base(handle)
    {
    }

    internal static H5DateTimeOffsetType Create()
    {
        var type = H5TAdapter.CreateCompoundType<Proxy, H5DateTimeOffsetType>(static h => new H5DateTimeOffsetType(h));

        try
        {
            type.Insert<Proxy, long>(nameof(Proxy.DateTime));
            type.Insert<Proxy, int>(nameof(Proxy.Offset));
            return type;
        }
        catch
        {
            type.Dispose();
            throw;
        }
    }

    internal struct Proxy
    {
        public long DateTime;
        public short Offset;
    }

    internal static DateTimeOffset FromProxy(Proxy proxy)
    {
        return new DateTimeOffset(
            DateTime.FromBinary(proxy.DateTime), 
            TimeSpan.FromMinutes(proxy.Offset));
    }

    internal static Proxy ToProxy(DateTimeOffset value)
    {
        return new H5DateTimeOffsetType.Proxy
        {
            DateTime = value.DateTime.ToBinary(),
            Offset = (short)value.Offset.TotalMinutes
        };
    }
}
