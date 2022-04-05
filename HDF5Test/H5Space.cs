using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5Space : H5SpaceHandle
    {
        internal H5Space(Handle handle) : base(handle)
        {

        }

        public void SelectHyperslab(int offset, int count)
        {
            SelectHyperslab(this, offset, count);
        }

        public static H5Space CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
        {
            Handle h = H5S.create_simple(rank, dims, maxdims);
            AssertHandle(h);
            return new H5Space(h);
        }

        public static void SelectHyperslab(H5SpaceHandle spaceId, int offset, int count)
        {
            int err = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { (ulong)offset }, null, new ulong[] { (ulong)count }, null);
            AssertError(err);
        }
    }
}
