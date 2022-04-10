namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5G (Group) API.
    /// </summary>
    public class H5Group : H5GroupHandle
    {
        private H5Group(Handle handle) : base(handle) { }

        /// <summary>
        /// Create a sub-Group of this group
        /// </summary>
        public override H5Group CreateGroup(string name)
        {
            return Create(this, name);
        }

        /// <summary>
        /// Create a DataSet for this Group
        /// </summary>
        public override H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        #region Factory methods
        public static H5Group Create(IH5Location location, string name)
        {
            AssertHandle(location.Handle);
            var h = H5G.create(location.Handle, name);
            AssertHandle(h);
            return new H5Group(h);
        }
        #endregion
    }
}
