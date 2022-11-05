namespace HDF5.Api.H5Types;

#if NET7_0_OR_GREATER

public class H5TimeOnlyType : H5Type<TimeOnly>
{
    internal H5TimeOnlyType(long handle) : base(handle)
    {
    }
}

#endif
