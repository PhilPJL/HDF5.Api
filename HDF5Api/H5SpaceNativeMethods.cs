using System.Collections.Generic;
using System.Linq;
using static HDF.PInvoke.H5S;

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

    /// <summary>
    /// Creates a new simple dataspace and opens it for access.
    /// </summary>
    /// <param name="rank">Number of dimensions of dataspace.</param>
    /// <param name="dims">Array specifying the size of each dimension.</param>
    /// <param name="maxdims">Array specifying the maximum size of each
    /// dimension.</param>
    /// <returns>Returns a dataspace identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Screate_simple")]
    [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t H5Screate_simple(int rank,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] hsize_t[] dims,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] hsize_t[] maxdims);

    public static H5Space CreateSimple(params Dimension[] dimensions)
    {
        long h = H5Screate_simple(dimensions.Length,
            dimensions.Select(d => d.InitialSize).ToArray(),
            dimensions.Select(d => d.UpperLimit).ToArray());

        h.ThrowIfInvalidHandleValue(nameof(H5Screate_simple));
        return new H5Space(h);
    }

    #endregion

    #region SelectHyperslab

    /// <summary>
    /// Selects a hyperslab region to add to the current selected region.
    /// </summary>
    /// <param name="space_id">Identifier of dataspace selection to modify</param>
    /// <param name="op">Operation to perform on current selection.</param>
    /// <param name="start">Offset of start of hyperslab</param>
    /// <param name="stride">Number of blocks included in hyperslab.</param>
    /// <param name="count">Hyperslab stride.</param>
    /// <param name="block">Size of block in hyperslab.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_hyperslab")]
    [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t H5Sselect_hyperslab
        (hid_t space_id, seloper_t op,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[]? start,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[]? stride,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[]? count,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[]? block);

    //TODO: expose other params
    public static void SelectHyperSlab(H5Space space, int offset, int count)
    {
        int err = H5Sselect_hyperslab(
            space, seloper_t.SET, new[] { (ulong)offset }, null, new[] { (ulong)count }, null);

        err.ThrowIfError("H5S.select_hyperslab");
    }

    #endregion

    #region GetSimpleExtentNPoints

    /// <summary>
    /// Determines the number of elements in a dataspace.
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace object to query</param>
    /// <returns>Returns the number of elements in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_npoints")]
    [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(CallConvCdecl) })]
    public static partial hssize_t H5Sget_simple_extent_npoints(hid_t space_id);

    public static long GetSimpleExtentNPoints(H5Space space)
    {
        long v = H5Sget_simple_extent_npoints(space);
        v.ThrowIfError(nameof(H5Sget_simple_extent_npoints));
        return v;
    }

    #endregion

    #region GetSimpleExtentDims

    /// <summary>
    /// Determines the dimensionality of a dataspace.
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace</param>
    /// <returns>Returns the number of dimensions in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_ndims")]
    [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(CallConvCdecl) })]
    public static partial int H5Sget_simple_extent_ndims(hid_t space_id);

    public static int GetSimpleExtentNDims(H5Space space)
    {
        int rank = H5Sget_simple_extent_ndims(space);

        rank.ThrowIfError(nameof(H5Sget_simple_extent_ndims));

        return rank;
    }

    /// <summary>
    /// Retrieves dataspace dimension size and maximum size.
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace object to query</param>
    /// <param name="dims">Pointer to array to store the size of each dimension.</param>
    /// <param name="maxdims">Pointer to array to store the maximum size of each dimension.</param>
    /// <returns>Returns the number of dimensions in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    /// <remarks>Either or both of <paramref name="dims"/> and
    /// <paramref name="maxdims"/> may be <code>NULL</code>.</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_dims")]
    [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Sget_simple_extent_dims
        (hid_t space_id, hsize_t[] dims, hsize_t[] maxdims);

    // TODO: dims are in-out params - test
    public static IReadOnlyList<Dimension> GetSimpleExtentDims(H5Space space)
    {
        var rank = GetSimpleExtentNDims(space);
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        int err = H5Sget_simple_extent_dims(space, dims, maxDims);
        err.ThrowIfError(nameof(H5Sget_simple_extent_dims));

        return dims.Zip(maxDims).Select(d => new Dimension(d.First, d.Second)).ToList();
    }

    #endregion
}

// TODO: argument validation
public readonly struct Dimension
{
    public readonly ulong MaxLimit = ulong.MaxValue;

    public readonly ulong InitialSize { get; }
    public readonly ulong UpperLimit { get; }

    public Dimension(ulong initialSize, ulong? upperLimit = null)
    {
        InitialSize = initialSize;
        UpperLimit = upperLimit ?? MaxLimit;
    }
};
