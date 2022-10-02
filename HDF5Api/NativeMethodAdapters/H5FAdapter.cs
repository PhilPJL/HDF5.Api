using static HDF5Api.NativeMethods.H5F;

namespace HDF5Api.NativeMethodAdapters;

/// <summary>
/// H5 file native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_f.html"/>
/// </summary>
internal static class H5FAdapter
{
    #region Close

    public static void Close(H5File file)
    {
        int err = close(file);

        err.ThrowIfError(nameof(close));
    }

    #endregion

    #region Create

    public static H5File Create(string path, H5PropertyList? fileCreationPropertyList = null, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = create(path, ACC_TRUNC, fileCreationPropertyList, fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(create));

        return new H5File(h);
    }

    #endregion

    public static H5File Open(string path, bool readOnly, H5PropertyList? fileAccessPropertyList = null)
    {
        long h = open(path, (uint)(readOnly ? ACC_RDONLY : ACC_RDWR), fileAccessPropertyList);

        h.ThrowIfInvalidHandleValue(nameof(open));

        return new H5File(h);
    }


    public static long GetObjectCount(H5File file, H5ObjectTypes types = H5ObjectTypes.All)
    {
#if NETSTANDARD
        return (long)get_obj_count(file, (uint)types);
#else
        return get_obj_count(file, (uint)types);
#endif
    }
}
