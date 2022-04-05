using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5PropertyList
    {
        public static H5PropertyListHandle Create(long classId)
        {
            Handle h = H5P.create(classId);
            H5Handle.AssertHandle(h);
            return new H5PropertyListHandle(h);
        }

        public static void SetChunk(H5PropertyListHandle handle, int rank, ulong[] dims)
        {
            H5Handle.AssertHandle(handle);
            int err = H5P.set_chunk(handle, rank, dims);
            H5Handle.AssertError(err);
        }

        public static void EnableDeflateCompression(H5PropertyListHandle handle, uint level)
        {
            H5Handle.AssertHandle(handle);
            int err = H5P.set_deflate(handle, level);
            H5Handle.AssertError(err);
        }
    }
}
