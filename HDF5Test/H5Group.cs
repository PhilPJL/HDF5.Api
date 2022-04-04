using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5Group
    {
        public static H5GroupHandle Create(H5LocationHandle handle, string name)
        {
            H5Handle.AssertHandle(handle);
            Handle h = H5G.create(handle, name);
            H5Handle.AssertHandle(h);
            return new H5GroupHandle(h);
        }
    }
}
