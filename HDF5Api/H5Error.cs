using System.Collections.Generic;
using HDF5Api.NativeMethodAdapters;

namespace HDF5Api;

/// <summary>
///     <para>.NET wrapper for H5E (Error) API.</para>
///     Native methods are described here: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_e.html"/>
/// </summary>
public static class H5Error
{
    /// <summary>
    ///     Stop HDF printing errors to the console
    /// </summary>
    public static void SetAutoOff()
    {
        H5EAdapter.SetAutoOff();
    }

    public static ICollection<H5ErrorInfo> WalkStack()
    {
        return H5EAdapter.WalkStack();
    }
}
