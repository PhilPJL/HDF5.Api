namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5G (Group) API.
    /// </summary>
    public class H5Group : H5Location<H5GroupHandle>
    {
        private H5Group(Handle handle) : base(new H5GroupHandle(handle)) { }

        #region Factory methods
        public static H5Group Create(H5LocationHandle locationId, string name)
        {
            locationId.ThrowIfNotValid();

            var h = H5G.create(locationId.Handle, name);
            
            h.ThrowIfNotValid();
            
            return new H5Group(h);
        }
        #endregion
    }
}
