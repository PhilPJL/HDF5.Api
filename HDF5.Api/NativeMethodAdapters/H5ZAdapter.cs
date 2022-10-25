using static HDF5.Api.NativeMethods.H5Z;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_z.html"/>
/// </summary>
internal static class H5ZAdapter
{
    internal static bool IsFilterAvailable(FilterType filterType)
    {
        switch (filterType)
        {
            case FilterType.Deflate:
            case FilterType.Shuffle:
            case FilterType.Fletcher32:
            case FilterType.SZip:
            case FilterType.NBit:
            case FilterType.ScaleOffset:
                return filter_avail((filter_t)filterType).ThrowIfError() > 0;
            default:
                throw new InvalidEnumArgumentException(nameof(filterType), (int)filterType, typeof(FilterType));
        }
    }
}
