namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5F (File) API.
    /// </summary>
    public class H5File : H5Location<H5FileHandle>
    {
        private H5File(Handle handle) : base(new H5FileHandle(handle)) { }

        #region Factory methods
        /// <summary>
        /// Create a file
        /// </summary>
        public static H5File Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            // TODO: open/create etc
            var h = H5F.create(name, flags);

            h.ThrowIfNotValid();
            
            return new H5File(h);
        }
        #endregion
    }
}
