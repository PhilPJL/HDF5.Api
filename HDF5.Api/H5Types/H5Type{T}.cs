namespace HDF5.Api.H5Types;

internal abstract class H5Type<T> : H5Type
{
    internal H5Type(long handle) : base(handle)
    {
    }
}
