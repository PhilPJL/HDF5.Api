﻿using static HDF5.Api.NativeMethods.H5O;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_o.html"/>
/// </summary>
internal static class H5OAdapter
{
    internal static info_t GetInfoByName(long locationId, string name)
    {
        info_t oinfo = default;
        int err1 = get_info_by_name(locationId, name, ref oinfo);
        err1.ThrowIfError();
        return oinfo;
    }

    internal static info_t GetInfoByName<T>(H5Object<T> h5Object, string name) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        return GetInfoByName((long)h5Object, name);
    }

    internal static info_t GetInfo<T>(H5Object<T> h5Object) where T : H5Object<T>
    {
        h5Object.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet);

        info_t oinfo = default;
        int err = get_info(h5Object, ref oinfo);
        err.ThrowIfError();
        return oinfo;
    }
}