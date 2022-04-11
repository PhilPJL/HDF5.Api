using System;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5T (Type) API.
    /// </summary>
    public class H5Type : H5TypeHandle
    {
        private H5Type(Handle handle) : base(handle) { }

        public H5Type Insert(string name, int offset, long nativeTypeId)
        {
            Insert(this, name, new IntPtr(offset), nativeTypeId);
            return this;
        }

        public H5Type Insert(string name, int offset, H5TypeHandle dataTypeId)
        {
            Insert(this, name, new IntPtr(offset), dataTypeId);
            return this;
        }

        public H5Type Insert(string name, IntPtr offset, long nativeTypeId)
        {
            Insert(this, name, offset, nativeTypeId);
            return this;
        }

        public H5Type Insert(string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            Insert(this, name, offset, dataTypeId);
            return this;
        }

        public H5Type Insert<S>(string name, long nativeTypeId) where S : struct
        {
            var offset = Marshal.OffsetOf<S>(name);
            Insert(this, name, offset, nativeTypeId);
            return this;
        }

        public H5Type Insert<S>(string name, H5TypeHandle dataTypeId) where S : struct
        {
            var offset = Marshal.OffsetOf<S>(name);
            Insert(this, name, offset, dataTypeId);
            return this;
        }

        #region Factory methods
        public static H5Type CreateCompoundType(int size)
        {
            var h = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateCompoundType<S>() where S : struct
        {
            int size = Marshal.SizeOf<S>();
            return CreateCompoundType(size);
        }

        public static H5Type CreateByteArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_B8, 1, new ulong[] { (ulong)size });
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateDoubleArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_DOUBLE, 1, new ulong[] { (ulong)size });
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateFloatArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_FLOAT, 1, new ulong[] { (ulong)size });
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateVariableLengthByteArrayType()
        {
            var h = H5T.vlen_create(H5T.NATIVE_B8);
            AssertHandle(h);
            return new H5Type(h);
        }

        public static H5Type CreateFixedLengthStringType(int length)
        {
            var h = H5T.copy(H5T.C_S1);
            AssertHandle(h);
            int err = H5T.set_size(h, new IntPtr(length));
            AssertError(err);
            return new H5Type(h);
        }

        #endregion

        #region C API wrappers
        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, long nativeTypeId)
        {
            AssertHandle(typeId);
            var err = H5T.insert(typeId, name, offset, nativeTypeId);
            AssertError(err);
        }

        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            AssertHandle(typeId);
            AssertHandle(dataTypeId);
            var err = H5T.insert(typeId, name, offset, dataTypeId);
            AssertError(err);
        }
        #endregion
    }
}
