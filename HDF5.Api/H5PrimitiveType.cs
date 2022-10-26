namespace HDF5.Api;

public class H5PrimitiveType<T> : H5Type<T> where T : unmanaged
{ 
    internal H5PrimitiveType(long handle) : base(handle)
    {
    }
}
