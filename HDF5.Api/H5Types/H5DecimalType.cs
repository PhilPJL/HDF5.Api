using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5DecimalType : H5Type<decimal>
{
    internal H5DecimalType(long handle) : base(handle)
    {
    }

    internal static H5DecimalType CreateType()
    {
        // TODO: 4 x long opaque type
        throw new NotImplementedException("decimal type: TODO.");
    }
}
