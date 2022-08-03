namespace HDF5Api;

/// <summary>
///     Wrapper for H5S (Space) API.
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

    public int GetSimpleExtentNDims()
    {
        return GetSimpleExtentNDims(this);
    }

    public (int rank, ulong[] dims, ulong[] maxDims) GetSimpleExtentDims()
    {
        return GetSimpleExtentDims(this);
    }

    #region C API wrappers

    public static H5Space CreateSimple(int rank, ulong[] dims, ulong[] maxdims)
    {
        Handle h = H5S.create_simple(rank, dims, maxdims);
        h.ThrowIfNotValid("H5S.create_simple");
        return new H5Space(h);
    }

    public static void SelectHyperslab(H5SpaceHandle spaceId, int offset, int count)
    {
        int err = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new[] { (ulong)offset }, null,
            new[] { (ulong)count }, null);
        err.ThrowIfError("H5S.select_hyperslab");
    }

    public static long GetSimpleExtentNPoints(H5SpaceHandle spaceId)
    {
        spaceId.ThrowIfNotValid();
        return H5S.get_simple_extent_npoints(spaceId.Handle);
    }

    public static int GetSimpleExtentNDims(H5SpaceHandle spaceId)
    {
        spaceId.ThrowIfNotValid();
        return H5S.get_simple_extent_ndims(spaceId.Handle);
    }

    public static (int rank, ulong[] dims, ulong[] maxDims) GetSimpleExtentDims(H5SpaceHandle spaceId)
    {
        var rank = GetSimpleExtentNDims(spaceId);
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        spaceId.ThrowIfNotValid();
        int err = H5S.get_simple_extent_dims(spaceId.Handle, dims, maxDims);
        err.ThrowIfError("H5S.get_simple_extent_dims");

        return (rank, dims, maxDims);
    }

    #endregion
}
