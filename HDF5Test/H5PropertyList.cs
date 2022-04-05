using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5PropertyList : H5PropertyListHandle
    {
        private H5PropertyList(Handle handle) : base(handle)
        {
        }

        public void SetChunk(int rank, ulong[] dims)
        {
            SetChunk(this, rank, dims);
        }

        public void EnableDeflateCompression(uint level)
        {
            EnableDeflateCompression(this, level);
        }

        #region Factory methods
        public static H5PropertyList Create(long classId)
        {
            Handle h = H5P.create(classId);
            AssertHandle(h);
            return new H5PropertyList(h);
        }
        #endregion

        #region C API wrappers
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
        #endregion
    }
}
