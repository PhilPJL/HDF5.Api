namespace HDF5.Api.H5Types;

public class H5DecimalType : H5Type<decimal>
{
    internal H5DecimalType(long handle) : base(handle)
    {
    }
}
