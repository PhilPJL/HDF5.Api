namespace HDF5.Api.H5Types;

public class H5PrimitiveType<T> : H5Type<T> //where T : unmanaged
{
    internal H5PrimitiveType(long handle) : base(handle)
    {
        H5ThrowHelpers.ThrowIfManaged<T>();
    }

    internal static H5PrimitiveType<T> Create()
    {
        return GetEquivalentNativeType<T>();
    }
}
