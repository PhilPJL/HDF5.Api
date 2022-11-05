namespace HDF5.Api.H5Types;

public class H5TimeSpanType : H5Type<TimeSpan>
{
    internal H5TimeSpanType(long handle) : base(handle)
    {
    }
}
