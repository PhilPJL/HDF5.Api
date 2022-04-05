using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5DataSet : H5DataSetHandle
    {
        private H5DataSet(Handle handle) : base(handle)
        {
        }

        public void Write(H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            Write(this, typeId, memorySpaceId, fileSpaceId, buffer);
        }

        public H5Space GetFileSpace()
        {
            return GetFileSpace(this);
        }

        #region Static C level API wrappers
        public static H5DataSet Create(IH5Location location, string name,
            Handle rawRecordTypeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            AssertHandle(location.Handle);
            AssertHandle(rawRecordTypeId);
            AssertHandle(spaceId);
            AssertHandle(propertyListId);
            Handle h = H5D.create(location.Handle, name, rawRecordTypeId, spaceId, H5P.DEFAULT, propertyListId);
            AssertHandle(h);
            return new H5DataSet(h);
        }

        public static H5Space GetFileSpace(H5DataSetHandle dataSetId)
        {
            AssertHandle(dataSetId);
            Handle h = H5D.get_space(dataSetId);
            AssertHandle(h);
            return new H5Space(h);
        }

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
