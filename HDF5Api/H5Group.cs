namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5G (Group) API.
    /// </summary>
    public class H5Group : H5Location<H5GroupHandle>
    {
        private H5Group(Handle handle) : base(new H5GroupHandle(handle)) { }

        #region C level API wrappers

        public static H5Group Create(H5LocationHandle locationId, string name)
        {
            locationId.ThrowIfNotValid();

            var h = H5G.create(locationId.Handle, name);

            h.ThrowIfNotValid("H5G.create");

            return new H5Group(h);
        }

        public static H5Group Open(H5LocationHandle locationId, string name)
        {
            locationId.ThrowIfNotValid();

            var h = H5G.open(locationId.Handle, name);

            h.ThrowIfNotValid("H5G.open");

            return new H5Group(h);
        }

        #endregion
    }
}
