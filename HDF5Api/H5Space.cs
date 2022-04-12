namespace HDF5Api
{
    /// <summary>
    /// Wrapper for H5S (Space) API.
    /// </summary>
    public class H5Space : H5Object<H5SpaceHandle>
    {
        private H5Space(Handle handle) : base(new H5SpaceHandle(handle)) { }

        public void SelectHyperslab(int offset, int count)
        {
            SelectHyperslab(this, offset, count);
        }

        #region Factory methods
        public static H5Space CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
        {
            var h = H5S.create_simple(rank, dims, maxdims);
            h.ThrowIfNotValid();
            return new H5Space(h);
        }
        #endregion

        #region C API wrappers
        public static void SelectHyperslab(H5SpaceHandle spaceId, int offset, int count)
        {
            var err = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { (ulong)offset }, null, new ulong[] { (ulong)count }, null);
            err.ThrowIfError();
        }

        public static H5Space GetDataSetSpace(H5DataSetHandle dataSetId)
        {
            dataSetId.ThrowIfNotValid();
            var h = H5D.get_space(dataSetId);
            h.ThrowIfNotValid();
            return new H5Space(h);
        }
        #endregion
    }
}
