namespace HDF5.Api.H5Types;

public class H5DateTimeType : H5Type<DateTime>
{
    internal H5DateTimeType(long handle) : base(handle)
    {
    }
}
