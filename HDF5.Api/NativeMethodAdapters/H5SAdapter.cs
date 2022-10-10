using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5S;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_s.html"/>
/// </summary>
internal static class H5SAdapter
{
    public static void Close(H5Space attribute)
    {
        int err = close(attribute);

        err.ThrowIfError();
    }

    internal static H5Space CreateSimple(params Dimension[] dimensions)
    {
        long h = create_simple(dimensions.Length,
            dimensions.Select(d => d.InitialSize).ToArray(),
            dimensions.Select(d => d.UpperLimit).ToArray());

        h.ThrowIfInvalidHandleValue();
        return new H5Space(h);
    }

    internal static H5Space CreateSimple(params long[] dimensions)
    {
        return CreateSimple(dimensions.Select(d => new Dimension(d)).ToArray());
    }

    //TODO: expose other params
    internal static void SelectHyperslab(H5Space space, long offset, long count)
    {
        int err = select_hyperslab(
            space, seloper_t.SET, new[] { (ulong)offset }, null!, new[] { (ulong)count }, null!);

        err.ThrowIfError();
    }

    internal static long GetSimpleExtentNPoints(H5Space space)
    {
        long v = get_simple_extent_npoints(space);
        v.ThrowIfError();
        return v;
    }

    internal static int GetSimpleExtentNDims(H5Space space)
    {
        int rank = get_simple_extent_ndims(space);

        rank.ThrowIfError();

        return rank;
    }

    internal static IReadOnlyList<Dimension> GetSimpleExtentDims(H5Space space)
    {
        var rank = GetSimpleExtentNDims(space);
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        int err = get_simple_extent_dims(space, dims, maxDims);
        err.ThrowIfError();

        return Enumerable.Zip(dims, maxDims, (f, s) => new Dimension(f, s)).ToList();
    }
}
