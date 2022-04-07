using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5Type : H5TypeHandle
    {
        private H5Type(Handle handle) : base(handle) { }

        public void Insert(string name, int offset, long nativeTypeId)
        {
            Insert(this, name, new IntPtr(offset), nativeTypeId);
        }

        public void Insert(string name, IntPtr offset, long nativeTypeId)
        {
            Insert(this, name, offset, nativeTypeId);
        }

        public void Insert(string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            Insert(this, name, offset, dataTypeId);
        }

        #region Factory methods
        public static H5Type CreateCompoundType(int size)
        {
            Handle h = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateByteArrayType(int size)
        {
            Handle h = H5T.array_create(H5T.NATIVE_B8, 1, new ulong[] { (ulong)size });
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateDoubleArrayType(int size)
        {
            Handle h = H5T.array_create(H5T.NATIVE_DOUBLE, 1, new ulong[] { (ulong)size });
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateVariableLengthByteArrayType()
        {
            Handle h = H5T.vlen_create(H5T.NATIVE_B8);
            AssertHandle(h);
            return new H5Type(h);
        }
        #endregion

        #region C API wrappers
        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, long nativeTypeId)
        {
            AssertHandle(typeId);
            int err = H5T.insert(typeId, name, offset, nativeTypeId);
            AssertError(err);
        }

        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            AssertHandle(typeId);
            AssertHandle(dataTypeId);
            int err = H5T.insert(typeId, name, offset, dataTypeId);
            AssertError(err);
        }
        #endregion
    }
}
