using System;

namespace HDF5Api;

public static class H5Error
{
    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        int err = H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
        err.ThrowIfError("H5E.set_auto");
    }
}
