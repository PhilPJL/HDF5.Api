﻿using static HDF5Api.NativeMethods.H5;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5 (Global) API.
/// </summary>
public static class H5Global
{
    public static Version GetLibraryVersion()
    {
        uint major = 0;
        uint minor = 0;
        uint revision = 0;

        int err = get_libversion(ref major, ref minor, ref revision);

        err.ThrowIfError(nameof(get_libversion));

        return new Version((int)major, (int)minor, 0, (int)revision);
    }

    /// <summary>
    ///     Is the referenced library built with thread-safe option?
    /// </summary>
    public static bool IsThreadSafe()
    {
        uint is_ts = 0;
        int err = is_library_threadsafe(ref is_ts);

        err.ThrowIfError(nameof(is_library_threadsafe));

        return is_ts != 0;
    }
}
