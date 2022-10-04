using static HDF5Api.NativeMethods.H5E;

namespace HDF5Api.NativeMethodAdapters;

internal static class H5EAdapter
{
    public static void SetAutoOff()
    {
        int err = set_auto(DEFAULT, null!, IntPtr.Zero);
        err.ThrowIfError(nameof(set_auto));
    }
}

