namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5F (File) API.
    /// </summary>
    public class H5File : H5FileHandle
    {
        private H5File(Handle handle) : base(handle) { }

        /// <summary>
        /// Create a Group in this file
        /// </summary>
        public override H5Group CreateGroup(string name)
        {
            return H5Group.Create(this, name);
        }

        /// <summary>
        /// Create a DataSet in this File
        /// </summary>
        public override H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        #region Factory methods
        /// <summary>
        /// Create a file
        /// </summary>
        public static H5File Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            // TODO: open/create etc
            var h = H5F.create(name, flags);
            AssertHandle(h);
            return new H5File(h);
        }
        #endregion
    }
}
