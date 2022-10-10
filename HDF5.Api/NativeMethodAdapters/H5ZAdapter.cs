using System.ComponentModel;
using static HDF5.Api.NativeMethods.H5Z;

namespace HDF5.Api.NativeMethodAdapters;

internal static class H5ZAdapter
{
    public static bool IsFilterAvailable(FilterType filterType)
    {
        switch (filterType)
        {
            case FilterType.Deflate:
            case FilterType.Shuffle:
            case FilterType.Fletcher32:
            case FilterType.SZip:
            case FilterType.NBit:
            case FilterType.ScaleOffset:
                int err = filter_avail((filter_t)filterType);
                err.ThrowIfError();
                return err > 0;
            default:
                throw new InvalidEnumArgumentException(nameof(filterType), (int)filterType, typeof(FilterType));
        }
    }
}
