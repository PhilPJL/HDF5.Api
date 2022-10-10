using HDF5.Api.NativeMethodAdapters;
using static HDF5.Api.NativeMethods.H5Z;

namespace HDF5.Api;

public static class H5Filter
{
    public static bool IsFilterAvailable(FilterType filterType)
    {
        return H5ZAdapter.IsFilterAvailable(filterType);
    }
}

public enum FilterType
{
    None = filter_t.NONE,
    Deflate = filter_t.DEFLATE,
    Shuffle = filter_t.SHUFFLE,
    Fletcher32 = filter_t.FLETCHER32,
    SZip = filter_t.SZIP,
    NBit = filter_t.NBIT,
    ScaleOffset = filter_t.SCALEOFFSET
}