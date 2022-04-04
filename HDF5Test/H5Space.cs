using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5Space
    {
        public static H5SpaceHandle CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
        {
            Handle h = H5S.create_simple(rank, dims, maxdims);
            H5Handle.AssertHandle(h);
            return new H5SpaceHandle(h);
        }

        public static void SelectHyperslab(H5SpaceHandle spaceId, int offset, int count)
        {
            int err = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { (ulong)offset }, null, new ulong[] { (ulong)count }, null);
            H5Handle.AssertError(err);
        }
    }
}
