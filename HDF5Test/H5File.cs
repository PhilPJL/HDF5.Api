using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5File
    {
        public static H5FileHandle Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            Handle h = H5F.create(name, flags);
            H5Handle.AssertHandle(h);
            return new H5FileHandle(h);
        }
    }
}
