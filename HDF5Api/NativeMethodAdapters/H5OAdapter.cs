using static HDF5Api.NativeMethods.H5O;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5OAdapter
{
    public static info_t GetInfoByName(long locationId, string name)
    {
        info_t oinfo = default;
        int err1 = get_info_by_name(locationId, name, ref oinfo);
        err1.ThrowIfError(nameof(get_info_by_name));
        return oinfo;
    }

    public static info_t GetInfoByName<T>(H5Object<T> location, string name) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        return GetInfoByName((long)location, name);
    }
}