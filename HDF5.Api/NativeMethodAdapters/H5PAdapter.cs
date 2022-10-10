using System.Linq;
using static HDF5.Api.NativeMethods.H5P;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 property list native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_p.html"/>
/// </summary>
internal static class H5PAdapter
{
    internal static void Close(H5PropertyList propertyList)
    {
        int err = close(propertyList);

        err.ThrowIfError();
    }

    internal static H5PropertyList Create(long classId)
    {
        long h = create(classId);

        h.ThrowIfInvalidHandleValue();

        return new H5PropertyList(h);
    }

    internal static void SetChunk(H5PropertyList propertyList, int rank, long[] dims)
    {
        int err = set_chunk(propertyList, rank, dims.Select(d => (ulong)d).ToArray());

        err.ThrowIfError();
    }

    internal static void EnableDeflateCompression(H5PropertyList propertyList, int level)
    {
        int err = set_deflate(propertyList, (uint)level);

        err.ThrowIfError();
    }

    internal static bool AreEqual(H5PropertyList propertyList1, H5PropertyList propertyList2)
    {
        int err = equal(propertyList1, propertyList2);

        err.ThrowIfError();

        return err != 0;
    }

    internal static H5PropertyList GetPropertyList<T>(H5Object<T> obj, Func<long, long> get_plist) where T : H5Object<T>
    {
        long h = get_plist(obj);

        h.ThrowIfInvalidHandleValue();

        return new H5PropertyList(h);
    }
}
