using HDF5.Api.NativeMethodAdapters;

namespace HDF5.Api.H5Types;

public class H5EnumType<T> : H5Type<T> // where T : unmanaged, Enum
{
    internal H5EnumType(long handle) : base(handle)
    {
    }

    // TODO: move enum specific members here

    internal static H5EnumType<T> Create()
    {
        return H5TAdapter.CreateEnumType<T, H5EnumType<T>>(static h => new H5EnumType<T>(h));
    }
}
