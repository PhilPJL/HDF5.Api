using static HDF5.Api.NativeMethods.H5Z;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_z.html"/>
/// </summary>
internal static class H5ZAdapter
{
    internal static bool IsFilterAvailable(FilterType filterType)
    {
        return filterType switch
        {
            FilterType.Deflate or FilterType.Shuffle or FilterType.Fletcher32 or FilterType.SZip or FilterType.NBit or FilterType.ScaleOffset
            => filter_avail((filter_t)filterType).ThrowIfError() > 0,
            _ => throw new InvalidEnumArgumentException(nameof(filterType), (int)filterType, typeof(FilterType)),
        };
    }
}
