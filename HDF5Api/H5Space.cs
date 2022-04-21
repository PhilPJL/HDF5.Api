namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5S (Space) API.
    /// </summary>
    public class H5Space : H5Object<H5SpaceHandle>
    {
        internal H5Space(Handle handle) : base(new H5SpaceHandle(handle)) { }

        public void SelectHyperslab(int offset, int count)
        {
            SelectHyperslab(this, offset, count);
        }

        public long GetSimpleExtentNPoints()
        {
            return GetSimpleExtentNPoints(this);
        }

        #region C API wrappers
        public static H5Space CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
        {
            var h = H5S.create_simple(rank, dims, maxdims);
            h.ThrowIfNotValid("H5S.create_simple");
            return new H5Space(h);
        }

        public static void SelectHyperslab(H5SpaceHandle spaceId, int offset, int count)
        {
            var err = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { (ulong)offset }, null, new ulong[] { (ulong)count }, null);
            err.ThrowIfError("H5S.select_hyperslab");
        }

        public static long GetSimpleExtentNPoints(H5SpaceHandle spaceId)
        {
            spaceId.ThrowIfNotValid();
            return H5S.get_simple_extent_npoints(spaceId.Handle);
        }
        #endregion
    }
}
