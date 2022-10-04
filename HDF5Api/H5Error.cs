using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

public static class H5Error
{
    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        H5EAdapter.SetAutoOff();
    }
}
