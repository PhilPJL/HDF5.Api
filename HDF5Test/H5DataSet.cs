using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5DataSet : H5DataSetHandle
    {
        private H5DataSet(Handle handle) : base(handle) { }

        public void Write(H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            Write(this, typeId, memorySpaceId, fileSpaceId, buffer);
        }

        public H5Space GetSpace()
        {
            return H5Space.GetDataSetSpace(this);
        }

        public void SetExtent(ulong[] dims)
        {
            SetExtent(this, dims);
        }

        #region Factory methods
        public static H5DataSet Create(IH5Location location, string name,
            H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            AssertHandle(location.Handle);
            AssertHandle(typeId);
            AssertHandle(spaceId);
            AssertHandle(propertyListId);
            Handle h = H5D.create(location.Handle, name, typeId, spaceId, H5P.DEFAULT, propertyListId);
            AssertHandle(h);
            return new H5DataSet(h);
        }
        #endregion

        #region C level API wrappers

        public static void SetExtent(H5DataSetHandle dataSetId, ulong[] dims)
        {
            AssertHandle(dataSetId);
            int err = H5D.set_extent(dataSetId, dims);
            AssertError(err);
        }

        public static void Write(H5DataSetHandle dataSetId, H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            AssertHandle(dataSetId);
            AssertHandle(typeId);
            AssertHandle(memorySpaceId);
            AssertHandle(fileSpaceId);
            int err = H5D.write(dataSetId, typeId, memorySpaceId, fileSpaceId, H5P.DEFAULT, buffer);
            AssertError(err);
        }
        #endregion
    }
}
