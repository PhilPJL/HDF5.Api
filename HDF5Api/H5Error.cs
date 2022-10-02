﻿using HDF5Api.NativeMethods;

namespace HDF5Api;

public static partial class H5Error
{
    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        int err = H5E.set_auto(H5E.DEFAULT, null!, IntPtr.Zero);
        err.ThrowIfError(nameof(H5E.set_auto));
    }
}
