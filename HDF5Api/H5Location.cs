namespace HDF5Api
{
    public interface IH5Location
    {
        public H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);
        public H5Group CreateGroup(string name);
        public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId);

    }

    public abstract class H5Location<THandle> : H5Object<THandle>, IH5Location where THandle : H5LocationHandle
    {
        protected H5Location(THandle handle) : base(handle) { }

        /// <summary>
        /// Create an Attribute for this File
        /// </summary>
        public H5Attribute CreateAttribute(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5Attribute.Create(Handle, name, typeId, spaceId, propertyListId);
        }

        /// <summary>
        /// Create a Group in this file
        /// </summary>
        public H5Group CreateGroup(string name)
        {
            return H5Group.Create(Handle, name);
        }

        /// <summary>
        /// Create a DataSet in this File
        /// </summary>
        public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }
    }
}
