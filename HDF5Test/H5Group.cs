using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5Group : H5GroupHandle
    {
        internal H5Group(Handle handle) : base(handle)
        {
        }

        public static H5GroupHandle Create(IH5Location location, string name)
        {
            AssertHandle(location.Handle);
            Handle h = H5G.create(location.Handle, name);
            AssertHandle(h);
            return new H5Group(h);
        }
    }
}
