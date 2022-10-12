using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using static HDF5.Api.NativeMethods.H5L;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_l.html"/>
/// </summary>
internal static class H5LAdapter
{
    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null)
        where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = exists(location, name, linkAccessPropertyList);

        err.ThrowIfError();

        return err > 0;
    }

    internal static void Delete<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null)
        where T : H5Object<T>

    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = delete(location, name, linkAccessPropertyList);

        err.ThrowIfError();
    }

    private static IEnumerable<string> GetChildNames<T>(H5Location<T> location, NativeMethods.H5O.type_t type)
        where T : H5Object<T>
    {
        ulong idx = 0;

        var names = new List<string>();

        int err = iterate(location, NativeMethods.H5.index_t.NAME, NativeMethods.H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
        err.ThrowIfError();

        return names;

        int Callback(long locationId, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            string? name = Marshal.PtrToStringAnsi(intPtrName);
#if DEBUG
            Guard.IsNotNull(name);
#endif
            if (name != null)
            {
                var oinfo = H5OAdapter.GetInfoByName(locationId, name);

                if (oinfo.type == type)
                {
                    names.Add(name);
                }
            }

            return 0;
        }
    }

    internal static IEnumerable<string> GetGroupNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetChildNames(location, NativeMethods.H5O.type_t.GROUP);

    internal static IEnumerable<string> GetDataSetNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetChildNames(location, NativeMethods.H5O.type_t.DATASET);

    internal static IEnumerable<string> GetNamedDataTypeNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetChildNames(location, NativeMethods.H5O.type_t.NAMED_DATATYPE);
}

