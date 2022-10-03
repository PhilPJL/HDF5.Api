using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using static HDF5Api.NativeMethods.H5S;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5SAdapter
{
    public static void Close(H5Space attribute)
    {
        int err = close(attribute);

        err.ThrowIfError(nameof(close));
    }

    public static H5Space CreateSimple(params Dimension[] dimensions)
    {
        long h = create_simple(dimensions.Length,
            dimensions.Select(d => d.InitialSize).ToArray(),
            dimensions.Select(d => d.UpperLimit).ToArray());

        h.ThrowIfInvalidHandleValue(nameof(create_simple));
        return new H5Space(h);
    }

    //TODO: expose other params
    public static void SelectHyperslab(H5Space space, long offset, long count)
    {
        int err = select_hyperslab(
            space, seloper_t.SET, new[] { (ulong)offset }, null!, new[] { (ulong)count }, null!);

        err.ThrowIfError(nameof(select_hyperslab));
    }

    public static long GetSimpleExtentNPoints(H5Space space)
    {
        long v = get_simple_extent_npoints(space);
        v.ThrowIfError(nameof(get_simple_extent_npoints));
        return v;
    }

    public static int GetSimpleExtentNDims(H5Space space)
    {
        int rank = get_simple_extent_ndims(space);

        rank.ThrowIfError(nameof(get_simple_extent_ndims));

        return rank;
    }

    public static IReadOnlyList<Dimension> GetSimpleExtentDims(H5Space space)
    {
        var rank = GetSimpleExtentNDims(space);
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        int err = get_simple_extent_dims(space, dims, maxDims);
        err.ThrowIfError(nameof(get_simple_extent_dims));

        return Enumerable.Zip(dims, maxDims, (f, s) => new Dimension(f, s)).ToList();
    }
}

public readonly struct Dimension
{
    public const ulong MaxLimit = ulong.MaxValue;

    public readonly ulong InitialSize { get; }
    public readonly ulong UpperLimit { get; }

    public Dimension(long initialSize, long? upperLimit = null)
    {
        Guard.IsGreaterThanOrEqualTo(initialSize, 0);
        Guard.IsGreaterThanOrEqualTo(upperLimit ?? 0, 0);

        InitialSize = (ulong)initialSize;

        if (upperLimit == null)
        {
            UpperLimit = MaxLimit;
        }
        else
        {
            UpperLimit = (ulong)upperLimit.Value;
        }
    }

    public Dimension(ulong initialSize, ulong? upperLimit = null)
    {
        InitialSize = initialSize;

        UpperLimit = upperLimit ?? MaxLimit;
    }
};

public static class DimensionExtensions
{
    
}