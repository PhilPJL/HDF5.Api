using HDF.PInvoke;
using System;
using Handle = System.Int64;

namespace HDF5Test
{
    public static class H5Type
    {
        public static H5TypeHandle CreateCompoundType(int size)
        {
            Handle h = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
            H5Handle.AssertHandle(h);
            return new H5TypeHandle(h);
        }

        public static void Insert(H5TypeHandle typeId, string name, int offset, long nativeTypeId)
        {
            H5T.insert(typeId, name, new IntPtr(offset), nativeTypeId);
        }

        public static void Insert(H5TypeHandle typeId, string name, IntPtr offset, long nativeTypeId)
        {
            H5Handle.AssertHandle(typeId);
            int err = H5T.insert(typeId, name, offset, nativeTypeId);
            H5Handle.AssertError(err);
        }
    }
}
