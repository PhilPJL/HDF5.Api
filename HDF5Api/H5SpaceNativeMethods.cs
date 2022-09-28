using System.Linq;

namespace HDF5Api;

internal static partial class H5SpaceNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial int H5Sclose(long handle);

    public static void Close(H5Space attribute)
    {
        int err = H5Sclose(attribute);

        err.ThrowIfError("H5Sclose");
    }

    #endregion

    #region CreateSimple

    public static H5Space CreateSimple(params Dimension[] dimensions)
    {
        long h = H5S.create_simple(dimensions.Length, 
            dimensions.Select(d => d.InitialSize).ToArray(), 
            dimensions.Select(d => d.UpperLimit ?? ulong.MaxValue).ToArray());

        h.ThrowIfInvalidHandleValue("H5S.create_simple");
        return new H5Space(h);
    }

    #endregion

    #region SelectHyperslab

    public static void SelectHyperslab(H5Space space, int offset, int count)
    {
        int err = H5S.select_hyperslab(
            space, H5S.seloper_t.SET, new[] { (ulong)offset }, null, new[] { (ulong)count }, null);

        err.ThrowIfError("H5S.select_hyperslab");
    }

    #endregion

    #region GetSimpleExtentNPoints

    public static long GetSimpleExtentNPoints(H5Space space)
    {
        return H5S.get_simple_extent_npoints(space);
    }

    #endregion

    #region GetSimpleExtentDims

    public static int GetSimpleExtentNDims(H5Space space)
    {
        return H5S.get_simple_extent_ndims(space);
    }

    public static (int rank, ulong[] dims, ulong[] maxDims) GetSimpleExtentDims(H5Space space)
    {
        var rank = GetSimpleExtentNDims(space);
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        int err = H5S.get_simple_extent_dims(space, dims, maxDims);
        err.ThrowIfError("H5S.get_simple_extent_dims");

        return (rank, dims, maxDims);
    }

    #endregion
}

// TODO: argument validation
record struct Dimension(ulong InitialSize, ulong? UpperLimit = null);
