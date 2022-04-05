using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5File : H5FileHandle
    {
        private H5File(Handle handle) : base(handle)
        {
        }

        public H5DataSet CreateDataSet(string name, Handle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        public H5Group CreateGroup(string name)
        {
            return H5Group.Create(this, name);
        }

        public static H5File Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            Handle h = H5F.create(name, flags);
            AssertHandle(h);
            return new H5File(h);
        }
    }
}
