using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
            return GetSpace(this);
        }

        public void SetExtent(ulong[] dims)
        {
            SetExtent(this, dims);
        }

        public H5Type GetH5Type()
        {
            return H5Type.GetType(Handle);
        }

        /// <summary>
        /// Open an existing Attribute for this dataset
        /// </summary>
        public H5Attribute OpenAttribute(string name)
        {
            return H5Attribute.Open(Handle, name);
        }

        public IEnumerable<T> Read<T>() where T : unmanaged
        {
            using (var space = GetSpace())
            using (var type = GetH5Type())
            {
                var count = space.GetSimpleExtentNPoints();

                var cls = type.GetClass();
                if (cls != H5T.class_t.COMPOUND)
                {
                    throw new HDF5Exception($"DataSet is of class {cls} when expecting {H5T.class_t.COMPOUND}.");
                }

                var size = GetStorageSize(this);

                if (size != count * Marshal.SizeOf<T>())
                {
                    throw new HDF5Exception($"Attribute storage size is {size}, which does not match the expected size for {count} items of type {typeof(T).Name} of {count * Marshal.SizeOf<T>()}.");
                }

                unsafe
                {
                    var result = new T[count];
                    fixed (T* ptr = result)
                    {
                        int err = H5D.read(Handle, type.Handle, space.Handle, space.Handle, 0, new IntPtr(ptr));
                        err.ThrowIfError("H5A.read");
                        return result;
                    }
                }
            }
        }

        #region C level API wrappers

        public static long GetStorageSize(H5DataSetHandle dataSetId)
        {
            dataSetId.ThrowIfNotValid();
            return (long)H5D.get_storage_size(dataSetId.Handle);
        }

        public static H5DataSet Create(H5LocationHandle location, string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            location.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            spaceId.ThrowIfNotValid();
            propertyListId.ThrowIfNotValid();

            var h = H5D.create(location, name, typeId, spaceId, H5P.DEFAULT, propertyListId);

            h.ThrowIfNotValid("H5D.create");

            return new H5DataSet(h);
        }

        public static H5DataSet Open(H5LocationHandle location, string name)
        {
            location.ThrowIfNotValid();

            var h = H5D.open(location, name);

            h.ThrowIfNotValid("H5D.open");

            return new H5DataSet(h);
        }

        public static bool Exists(H5LocationHandle location, string name)
        {
            location.ThrowIfNotValid();

            var err = H5L.exists(location, name);

            err.ThrowIfError("H5L.exists");

            return err > 0;
        }

        public static void SetExtent(H5DataSetHandle dataSetId, ulong[] dims)
        {
            dataSetId.ThrowIfNotValid();

            var err = H5D.set_extent(dataSetId, dims);

            err.ThrowIfError("H5D.set_extent");
        }

        public static void Write(H5DataSetHandle dataSetId, H5TypeHandle typeId, H5SpaceHandle memorySpaceId, H5SpaceHandle fileSpaceId, IntPtr buffer)
        {
            dataSetId.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            memorySpaceId.ThrowIfNotValid();
            fileSpaceId.ThrowIfNotValid();

            var err = H5D.write(dataSetId, typeId, memorySpaceId, fileSpaceId, H5P.DEFAULT, buffer);

            err.ThrowIfError("H5D.write");
        }

        public static H5Space GetSpace(H5DataSetHandle dataSetId)
        {
            dataSetId.ThrowIfNotValid();

            var h = H5D.get_space(dataSetId);

            h.ThrowIfNotValid("H5D.get_space");

            return new H5Space(h);
        }

        #endregion
    }
}
