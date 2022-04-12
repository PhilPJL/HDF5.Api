using System;

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

        #region Factory methods
        public static H5Attribute Create(H5LocationHandle locationId, string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            locationId.ThrowIfNotValid();
            typeId.ThrowIfNotValid();
            spaceId.ThrowIfNotValid();
            propertyListId.ThrowIfNotValid();

            var h = H5A.create(locationId.Handle, name, typeId, spaceId, propertyListId);
            
            h.ThrowIfNotValid();
            
            return new H5Attribute(h);
        }
        #endregion

        #region C level API wrappers
        public static bool Exists(H5LocationHandle locationId, string name)
        {
            int err = H5A.exists(locationId, name);
            
            err.ThrowIfError();
            
            return err > 0;
        }

        public static void Write(H5AttributeHandle attributeId, H5TypeHandle typeId, IntPtr buffer)
        {
            int err = H5A.write(attributeId, typeId, buffer);

            err.ThrowIfError();
        }
        #endregion
    }
}
