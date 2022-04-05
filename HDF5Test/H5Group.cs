﻿using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5Group : H5GroupHandle
    {
        private H5Group(Handle handle) : base(handle)
        {
        }

        public H5DataSet CreateDataSet(string name, Handle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        public static H5Group Create(IH5Location location, string name)
        {
            AssertHandle(location.Handle);
            Handle h = H5G.create(location.Handle, name);
            AssertHandle(h);
            return new H5Group(h);
        }
    }
}
