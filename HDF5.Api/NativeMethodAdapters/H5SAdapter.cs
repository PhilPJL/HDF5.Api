#if NET7_0_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers;
#endif
using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5S;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_s.html"/>
/// </summary>
internal static unsafe class H5SAdapter
{
    public static void Close(H5Space attribute)
    {
        close(attribute).ThrowIfError();
    }

    internal static H5Space CreateSimple(params Dimension[] dimensions)
    {
        // NOTE: assuming .NET standard marshalling is doing the right thing here
        // pin?

        ulong[] dims = dimensions.Select(d => d.InitialSize).ToArray();
        ulong[] maxDims = dimensions.Select(d => d.UpperLimit).ToArray();

        return new H5Space(create_simple(dimensions.Length, dims, maxDims));
    }

    internal static H5Space CreateSimple(params long[] dimensions)
    {
        return CreateSimple(dimensions.Select(d => new Dimension(d)).ToArray());
    }

    internal static H5Space CreateScalar()
    {
        return new H5Space(create(class_t.SCALAR));
    }

    internal static void SelectHyperslab(H5Space space, long offset, long count)
    {
        // NOTE: assuming .NET standard marshalling is doing the right thing here
        // pin?

        select_hyperslab(space, seloper_t.SET, new[] { (ulong)offset }, null!, new[] { (ulong)count }, null!)
            .ThrowIfError();
    }

    internal static long GetSimpleExtentNPoints(H5Space space)
    {
        return get_simple_extent_npoints(space).ThrowIfError();
    }

    internal static int GetSimpleExtentNDims(H5Space space)
    {
        return get_simple_extent_ndims(space).ThrowIfError();
    }

    internal static IReadOnlyList<Dimension> GetSimpleExtentDims(H5Space space)
    {
        var rank = GetSimpleExtentNDims(space);

        if (rank == 0)
        {
            return Array.Empty<Dimension>();
        }

#if NET7_0_OR_GREATER
        if (rank > 255)
        {
            using var dimsOwner = SpanOwner<ulong>.Allocate(rank);
            using var maxDimsOwner = SpanOwner<ulong>.Allocate(rank);

            return GetSimpleExtentDims(dimsOwner.Span, maxDimsOwner.Span);
        }
        else
        {
            Span<ulong> dims = stackalloc ulong[rank];
            Span<ulong> maxDims = stackalloc ulong[rank];

            return GetSimpleExtentDims(dims, maxDims);
        }

        IReadOnlyList<Dimension> GetSimpleExtentDims(Span<ulong> dims, Span<ulong> maxDims)
        {
            get_simple_extent_dims(space, dims, maxDims).ThrowIfError();

            var dimensions = new List<Dimension>();
            for (int i = 0; i < rank; i++)
            {
                dimensions.Add(new Dimension(dims[i], maxDims[i]));
            }

            return dimensions;
        }
#endif

#if NETSTANDARD
        var dims = new ulong[rank];
        var maxDims = new ulong[rank];

        fixed (ulong* dimsPtr = dims)
        fixed (ulong* maxDimsPtr = maxDims)
        {
            int result = get_simple_extent_dims(space, dimsPtr, maxDimsPtr);
            result.ThrowIfError();
            return dims.Zip(maxDims, (f, s) => new Dimension(f, s)).ToList();
        }
#endif
    }
}
