namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5P (Property list) API.
    /// </summary>
    public class H5PropertyList : H5Object<H5PropertyListHandle>
    {
        private H5PropertyList(Handle handle) : base(new H5PropertyListHandle(handle)) { }

        public void SetChunk(int rank, ulong[] dims)
        {
            SetChunk(this, rank, dims);
        }

        public void EnableDeflateCompression(uint level)
        {
            EnableDeflateCompression(this, level);
        }

        #region Factory methods
        public static H5PropertyList Create(Handle classId)
        {
            var h = H5P.create(classId);

            h.ThrowIfNotValid();
            
            return new H5PropertyList(h);
        }
        #endregion

        #region C API wrappers
        public static void SetChunk(H5PropertyListHandle propertyListId, int rank, ulong[] dims)
        {
            propertyListId.ThrowIfNotValid();

            var err = H5P.set_chunk(propertyListId, rank, dims);
            
            err.ThrowIfError();
        }

        public static void EnableDeflateCompression(H5PropertyListHandle propertyListId, uint level)
        {
            propertyListId.ThrowIfNotValid();
            
            var err = H5P.set_deflate(propertyListId, level);
            
            err.ThrowIfError();
        }
        #endregion
    }
}
