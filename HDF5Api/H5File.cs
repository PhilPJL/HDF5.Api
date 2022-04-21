namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5F (File) API.
    /// </summary>
    public class H5File : H5Location<H5FileHandle>
    {
        private H5File(Handle handle) : base(new H5FileHandle(handle)) { }

        #region C level API wrappers

        /// <summary>
        /// Create a new file.
        /// </summary>
        public static H5File Create(string path)
        {
            var h = H5F.create(path, H5F.ACC_TRUNC);

            h.ThrowIfNotValid("H5F.create");

            return new H5File(h);
        }

        /// <summary>
        /// Open an existing file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static H5File OpenReadOnly(string path)
        {
            var h = H5F.open(path, H5F.ACC_RDONLY);

            h.ThrowIfNotValid("H5F.open");

            return new H5File(h);
        }
        
        #endregion
    }
}