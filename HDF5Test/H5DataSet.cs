using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5DataSet
    {
        public static H5DataSetHandle Create(H5LocationHandle location, string name,
            Handle rawRecordTypeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            H5Handle.AssertHandle(location);
            H5Handle.AssertHandle(rawRecordTypeId);
            H5Handle.AssertHandle(spaceId);
            H5Handle.AssertHandle(propertyListId);
            Handle h = H5D.create(location, name, rawRecordTypeId, spaceId, H5P.DEFAULT, propertyListId);
            H5Handle.AssertHandle(h);
            return new H5DataSetHandle(h);
        }

        public static H5SpaceHandle GetFileSpace(H5DataSetHandle dataSetId)
        {
            H5Handle.AssertHandle(dataSetId);
            Handle h = H5D.get_space(dataSetId);
            H5Handle.AssertHandle(h);
            return new H5SpaceHandle(h);
        }

        public static void SetExtent(H5DataSetHandle dataSetId, ulong[] dims)
        {
            H5Handle.AssertHandle(dataSetId);
            int err = H5D.set_extent(dataSetId, dims);
            H5Handle.AssertError(err);
        }

        public static void Write(H5DataSetHandle dataSetId, H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            H5Handle.AssertHandle(dataSetId);
            H5Handle.AssertHandle(typeId);
            H5Handle.AssertHandle(memorySpaceId);
            H5Handle.AssertHandle(fileSpaceId);
            int err = H5D.write(dataSetId, typeId, memorySpaceId, fileSpaceId, H5P.DEFAULT, buffer);
            H5Handle.AssertError(err);
        }
    }
}
