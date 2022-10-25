namespace HDF5.Api;

public class H5EnumType<T> : H5Type where T : unmanaged, Enum
{
    internal H5EnumType(long handle) : base(handle)
    {
    }

    // TODO: move enum specific members here
}
