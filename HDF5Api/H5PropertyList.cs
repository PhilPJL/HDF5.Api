namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5P (Property list) API.
    /// </summary>
    public class H5PropertyList : H5PropertyListHandle
    {
        private H5PropertyList(Handle handle) : base(handle) { }

        public void SetChunk(int rank, ulong[] dims)
        {
            SetChunk(this, rank, dims);
        }

        // TODO: other compression types?
        public void EnableDeflateCompression(uint level)
        {
            EnableDeflateCompression(this, level);
        }

        #region Factory methods
        public static H5PropertyList Create(long classId)
        {
            var h = H5P.create(classId);
            AssertHandle(h);
            return new H5PropertyList(h);
        }
        #endregion

        #region C API wrappers
        public static void SetChunk(H5PropertyListHandle handle, int rank, ulong[] dims)
        {
            AssertHandle(handle);
            var err = H5P.set_chunk(handle, rank, dims);
            AssertError(err);
        }

        public static void EnableDeflateCompression(H5PropertyListHandle handle, uint level)
        {
            AssertHandle(handle);
            var err = H5P.set_deflate(handle, level);
            AssertError(err);
        }
        #endregion
    }
}
