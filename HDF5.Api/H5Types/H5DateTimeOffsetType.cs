namespace HDF5.Api.H5Types;

public class H5DateTimeOffsetType : H5Type<DateTimeOffset>
{
    internal H5DateTimeOffsetType(long handle) : base(handle)
    {
    }
}
