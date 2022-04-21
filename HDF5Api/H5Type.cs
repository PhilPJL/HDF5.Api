using System;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5T (Type) API.
    /// </summary>
    public class H5Type : H5Object<H5TypeHandle>
    {
        private H5Type(Handle handle) : base(new H5TypeHandle(handle)) { }

        public H5Type Insert(string name, int offset, Handle nativeTypeId)
        {
            Insert(this, name, new IntPtr(offset), nativeTypeId);
            return this;
        }

        public H5Type Insert(string name, int offset, H5TypeHandle dataTypeId)
        {
            Insert(this, name, new IntPtr(offset), dataTypeId);
            return this;
        }

        public H5Type Insert(string name, IntPtr offset, Handle nativeTypeId)
        {
            Insert(this, name, offset, nativeTypeId);
            return this;
        }

        public H5Type Insert(string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            Insert(this, name, offset, dataTypeId);
            return this;
        }

        public H5Type Insert<S>(string name, Handle nativeTypeId) where S : struct
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

        public H5T.class_t GetClass()
        {
            return GetClass(this);
        }

        #region C API wrappers
        public static H5T.class_t GetClass(H5TypeHandle typeId)
        {
            return H5T.get_class(typeId.Handle);
        }

        public static H5Type CreateCompoundType(int size)
        {
            var h = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
            h.ThrowIfNotValid("H5T.create");
            return new H5Type(h);
        }

        /// <summary>
        /// Create a Compound type in order to hold an <typeparamref name="S"/>
        /// </summary>
        public static H5Type CreateCompoundType<S>() where S : struct
        {
            int size = Marshal.SizeOf<S>();
            return CreateCompoundType(size);
        }

        /// <summary>
        /// Create a Compound type in order to hold an <typeparamref name="S"/> plus additional space as defined by <paramref name="extraSpace"/>
        /// </summary>
        public static H5Type CreateCompoundType<S>(int extraSpace) where S : struct
        {
            int size = Marshal.SizeOf<S>() + extraSpace;
            return CreateCompoundType(size);
        }

        public static H5Type CreateByteArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_B8, 1, new ulong[] { (ulong)size });
            h.ThrowIfNotValid("H5T.array_create");
            return new H5Type(h);
        }

        public static H5Type CreateDoubleArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_DOUBLE, 1, new ulong[] { (ulong)size });
            h.ThrowIfNotValid("H5T.array_create");
            return new H5Type(h);
        }

        public static H5Type CreateFloatArrayType(int size)
        {
            var h = H5T.array_create(H5T.NATIVE_FLOAT, 1, new ulong[] { (ulong)size });
            h.ThrowIfNotValid("H5T.array_create");
            return new H5Type(h);
        }

        public static H5Type CreateVariableLengthByteArrayType()
        {
            var h = H5T.vlen_create(H5T.NATIVE_B8);
            h.ThrowIfNotValid("H5T.vlen_create");
            return new H5Type(h);
        }

        public static H5Type CreateFixedLengthStringType(int length)
        {
            var h = H5T.copy(H5T.C_S1);
            h.ThrowIfNotValid("H5T.copy");
            int err = H5T.set_size(h, new IntPtr(length));
            err.ThrowIfError("H5T.set_size");
            return new H5Type(h);
        }

        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, Handle nativeTypeId)
        {
            typeId.ThrowIfNotValid();
            var err = H5T.insert(typeId, name, offset, nativeTypeId);
            err.ThrowIfError("H5T.insert");
        }

        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, H5TypeHandle dataTypeId)
        {
            typeId.ThrowIfNotValid();
            dataTypeId.ThrowIfNotValid();
            var err = H5T.insert(typeId, name, offset, dataTypeId);
            err.ThrowIfError("H5T.insert");
        }

        public static H5Type GetType(H5AttributeHandle attributeId)
        {
            var h = H5A.get_type(attributeId);

            h.ThrowIfNotValid("H5A.get_type");

            return new H5Type(h);
        }

        #endregion
    }
}
