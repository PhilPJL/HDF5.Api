using HDF5.Api.Utils;
using static HDF5.Api.NativeMethods.H5I;
namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_i.html"/>
/// </summary>
internal static unsafe class H5IAdapter
{
    internal static string GetName<T>(H5Object<T> location) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.DataSet, HandleType.Group, HandleType.Type);

        return MarshalHelpers.GetName(location, get_name);
    }
}
