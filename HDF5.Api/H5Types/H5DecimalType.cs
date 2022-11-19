using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5DecimalType : H5Type<decimal>
{
    internal H5DecimalType(long handle) : base(handle)
    {
    }

    internal static H5DecimalType Create()
    {
        return H5TAdapter.CreateOpaqueType(sizeof(decimal), typeof(decimal).FullName!, static h => new H5DecimalType(h));
    }

    // TODO?
    //internal string Tag => H5TAdapter.GetOpaqueTypeTag(this);
}
