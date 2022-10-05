using HDF5Api.NativeMethods;
using static HDF5Api.NativeMethods.H5L;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5LAdapter
{
    public static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null)
        where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = exists(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5L.exists));

        return err > 0;
    }

    public static void Delete<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null)
        where T : H5Object<T>

    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = delete(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5L.delete));    
    }

    // TODO: iterate
}

