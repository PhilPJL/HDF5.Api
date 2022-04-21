using HDF5Api.Disposables;
using System;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5G (Attribute) API.
    /// </summary>
    public class H5Attribute : H5Object<H5AttributeHandle>
    {
        private H5Attribute(Handle handle) : base(new H5AttributeHandle(handle)) { }

        public void Write(H5TypeHandle typeId, IntPtr buffer)
        {
            Write(this, typeId, buffer);
        }

        public H5Type GetH5Type()
        {
            return H5Type.GetType(Handle);
        }

        public string ReadString()
        {
            using (var type = GetH5Type())
            using (var space = GetSpace())
            {
                var count = space.GetSimpleExtentNPoints();

                if (count != 1)
                {
                    throw new HDF5Exception("Attribute contains an array type (not supported).");
                }

                var cls = type.GetClass();
                if (cls != H5T.class_t.STRING)
                {
                    throw new HDF5Exception($"Attribute is of class {cls} when expecting STRING.");
                }

                var size = GetStorageSize(this);

                // TODO: probably simpler way to do this.
                using (var buffer = new GlobalMemory((int)(size + 1)))
                {
                    int err = H5A.read(Handle, type.Handle, buffer.IntPtr);
                    err.ThrowIfError();

                    // TODO: marshal Ansi/UTF8/.. etc as appropriate
                    return Marshal.PtrToStringAnsi(buffer.IntPtr, (int)size);
                }
            }
        }

        public T ReadPrimitive<T>() where T : unmanaged
        {
            using (var type = GetH5Type())
            using (var space = GetSpace())
            {
                var count = space.GetSimpleExtentNPoints();

                if (count != 1)
                {
                    throw new HDF5Exception("Attribute contains an array type (not supported).");
                }

                var cls = type.GetClass();

                //var expectedCls = GetH5ClassFromT<T>();

                //if (cls != expectedCls)
                //{
                //    throw new HDF5Exception($"Attribute is of class {cls} when expecting {expectedCls}.");
                //}

                var size = GetStorageSize(this);

                if (size != Marshal.SizeOf<T>())
                {
                    throw new HDF5Exception($"Attribute storage size is {size}, which does not match the expected size for type {typeof(T).Name} of {Marshal.SizeOf<T>()}.");
                }

                unsafe
                {
                    T result = default;
                    int err = H5A.read(Handle, type.Handle, new IntPtr(&result));
                    err.ThrowIfError("H5A.read");
                    return result;
                }
            }
        }

        public double ReadDouble() => ReadPrimitive<double>();
        public int ReadInt32() => ReadPrimitive<int>();

        public H5Space GetSpace()
        {
            return GetSpace(Handle);
        }

        #region C level API wrappers

        public static H5Attribute Create(H5LocationHandle locationId, string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            locationId.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            spaceId.ThrowIfNotValid();
            propertyListId.ThrowIfNotValid();

            var h = H5A.create(locationId.Handle, name, typeId, spaceId, propertyListId);

            h.ThrowIfNotValid("H5A.create");

            return new H5Attribute(h);
        }

        private static H5Attribute OpenAttribute(H5Handle attributeParentId, string name)
        {
            attributeParentId.ThrowIfNotValid();

            var h = H5A.open(attributeParentId.Handle, name);

            h.ThrowIfNotValid("H5A.open");

            return new H5Attribute(h);
        }

        public static H5Attribute Open(H5LocationHandle locationId, string name)
        {
            return OpenAttribute(locationId, name);
        }

        public static H5Attribute Open(H5DataSetHandle dataSetId, string name)
        {
            return OpenAttribute(dataSetId, name);
        }

        public static void Write(H5AttributeHandle attributeId, H5TypeHandle typeId, IntPtr buffer)
        {
            int err = H5A.write(attributeId, typeId, buffer);

            err.ThrowIfError("H5A.write");
        }

        public static H5Space GetSpace(H5AttributeHandle attributeId)
        {
            attributeId.ThrowIfNotValid();
            var h = H5A.get_space(attributeId.Handle);
            h.ThrowIfNotValid("H5A.get_space");
            return new H5Space(h);
        }

        public static long GetStorageSize(H5AttributeHandle attributeId)
        {
            attributeId.ThrowIfNotValid();
            return (long)H5A.get_storage_size(attributeId.Handle);
        }

        public static bool Exists(H5LocationHandle locationId, string name)
        {
            locationId.ThrowIfNotValid();
            var err = H5A.exists(locationId, name);
            err.ThrowIfError("H5A.exists");
            return err >= 0;
        }

        public static bool Exists(H5DataSetHandle dataSetId, string name)
        {
            dataSetId.ThrowIfNotValid();
            var err = H5A.exists(dataSetId, name);
            err.ThrowIfError("H5A.exists");
            return err >= 0;
        }

        #endregion
    }
}
