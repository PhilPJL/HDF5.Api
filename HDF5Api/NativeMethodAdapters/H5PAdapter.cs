using System.Linq;
using static HDF5Api.NativeMethods.H5P;

namespace HDF5Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
internal static class H5PAdapter
{
    public static void Close(H5PropertyList propertyList)
    {
        int err = close(propertyList);

        err.ThrowIfError(nameof(close));
    }

    public static H5PropertyList Create(long classId)
    {
        long h = create(classId);

        h.ThrowIfInvalidHandleValue(nameof(create));

        return new H5PropertyList(h);
    }

    public static void SetChunk(H5PropertyList propertyList, int rank, long[] dims)
    {
        int err = set_chunk(propertyList, rank, dims.Select(d => (ulong)d).ToArray());

        err.ThrowIfError(nameof(set_chunk));
    }

    public static void EnableDeflateCompression(H5PropertyList propertyList, int level)
    {
        int err = set_deflate(propertyList, (uint)level);

        err.ThrowIfError(nameof(set_deflate));
    }

    internal static H5PropertyList GetPropertyList<T>(H5Object<T> obj, Func<long, long> get_plist) where T : H5Object<T>
    {
        long h = get_plist(obj);

        h.ThrowIfInvalidHandleValue(nameof(get_plist));

        return new H5PropertyList(h);
    }
}
