using HDF5Api;
using HDF5Api.NativeMethods;
using static HDF5Api.NativeMethods.H5P;

internal static partial class H5PAdapter
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

    public static void SetChunk(H5PropertyList propertyList, int rank, ulong[] dims)
    {
        int err = set_chunk(propertyList, rank, dims);

        err.ThrowIfError(nameof(set_chunk));
    }

    public static void EnableDeflateCompression(H5PropertyList propertyList, uint level)
    {
        int err = set_deflate(propertyList, level);

        err.ThrowIfError(nameof(set_deflate));
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    public static H5PropertyList GetCreationPropertyList(H5DataSet dataSet)
    {
        long h = H5D.get_create_plist(dataSet);
        h.ThrowIfInvalidHandleValue(nameof(H5D.get_create_plist));
        return new H5PropertyList(h);
    }
}