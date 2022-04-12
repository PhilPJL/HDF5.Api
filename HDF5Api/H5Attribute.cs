using System;

namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5G (Attribute) API.
    /// </summary>
    public class H5Attribute : H5AttributeHandle
    {
        private H5Attribute(Handle handle) : base(handle) { }

        public void Write(H5SpaceHandle spaceId, IntPtr buffer)
        {
            Write(this, spaceId, buffer);
        }

        #region Factory methods
        public static H5Attribute Create(IH5Location location, string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            AssertHandle(location.Handle);
            var h = H5A.create(location.Handle, name, typeId, spaceId, propertyListId);
            AssertHandle(h);
            return new H5Attribute(h);
        }
        #endregion

        #region C level API wrappers
        public static bool Exists(IH5Location location, string name)
        {
            int err = H5A.exists(location.Handle, name);
            AssertError(err);
            return err > 0;
        }

        public static void Write(H5AttributeHandle handle, H5SpaceHandle spaceId, IntPtr buffer)
        {
            int err = H5A.write(handle, spaceId, buffer);
            AssertError(err);
        }
        #endregion
    }
}
