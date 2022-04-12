using System;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5D (Data-set) API.
    /// </summary>
    public class H5DataSet : H5Object<H5DataSetHandle>
    {
        private H5DataSet(Handle handle) : base(new H5DataSetHandle(handle)) { }

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
        public static H5DataSet Create(H5LocationHandle location, string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            location.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            spaceId.ThrowIfNotValid();
            propertyListId.ThrowIfNotValid();

            var h = H5D.create(location, name, typeId, spaceId, H5P.DEFAULT, propertyListId);
            
            h.ThrowIfNotValid();
            
            return new H5DataSet(h);
        }
        #endregion

        #region C level API wrappers
        public static void SetExtent(H5DataSetHandle dataSetId, ulong[] dims)
        {
            dataSetId.ThrowIfNotValid();

            var err = H5D.set_extent(dataSetId, dims);
            
            err.ThrowIfError();
        }

        public static void Write(H5DataSetHandle dataSetId, H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            dataSetId.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            memorySpaceId.ThrowIfNotValid();
            fileSpaceId.ThrowIfNotValid();

            var err = H5D.write(dataSetId, typeId, memorySpaceId, fileSpaceId, H5P.DEFAULT, buffer);
            
            err.ThrowIfError();
        }
        #endregion
    }
}
