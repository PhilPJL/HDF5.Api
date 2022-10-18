using CommunityToolkit.Diagnostics;
using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5L;
#if NETSTANDARD
using HDF5.Api.Utils;
#endif

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_l.html"/>
/// </summary>
internal static unsafe class H5LAdapter
{
    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList)
        where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        int err;

#if NET7_0_OR_GREATER
        err = exists(location, name, linkAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = exists(location, nameBytesPtr, linkAccessPropertyList);
        }
#endif

        err.ThrowIfError();

        return err > 0;
    }

    internal static void Delete<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList)
        where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        int err;

#if NET7_0_OR_GREATER
         err = delete(location, name, linkAccessPropertyList);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            err = delete(location, nameBytesPtr, linkAccessPropertyList);
        }
#endif

        err.ThrowIfError();
    }

    private static IEnumerable<(string name, H5ObjectType type)> GetMembers<T>(H5Location<T> location, H5O.type_t type)
        where T : H5Object<T>
    {
        ulong idx = 0;

        var names = new List<(string, H5ObjectType)>();

        int err = iterate(location, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

        err.ThrowIfError();

        return names;

        int Callback(long locationId, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            var name = info.cset switch
            {
#if NET7_0_OR_GREATER
                H5T.cset_t.ASCII or H5T.cset_t.UTF8 => Marshal.PtrToStringUTF8(intPtrName),
#else
                H5T.cset_t.ASCII or H5T.cset_t.UTF8 => MarshalHelpers.PtrToStringUTF8(intPtrName),
#endif
                _ => throw new InvalidEnumArgumentException($"Unexpected character set {info.cset} when enumerating attribute names."),
            };

            Guard.IsNotNull(name);

            var oinfo = H5OAdapter.GetInfoByName(locationId, name);

            if (oinfo.type == type || type == H5O.type_t.UNKNOWN)
            {
                names.Add((name, (H5ObjectType)oinfo.type));
            }

            return 0;
        }
    }

    internal static IEnumerable<string> GetGroupNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetMembers(location, H5O.type_t.GROUP).Select(m => m.name);

    internal static IEnumerable<string> GetDataSetNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetMembers(location, H5O.type_t.DATASET).Select(m => m.name);

    internal static IEnumerable<string> GetNamedDataTypeNames<T>(H5Location<T> location)
        where T : H5Object<T> => GetMembers(location, H5O.type_t.NAMED_DATATYPE).Select(m => m.name);

    internal static IEnumerable<(string name, H5ObjectType type)> GetMembers<T>(H5Location<T> location)
        where T : H5Object<T> => GetMembers(location, H5O.type_t.UNKNOWN);

    /// <summary>
    /// Create a new link creation property list
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    internal static H5LinkCreationPropertyList CreateCreationPropertyList(CharacterSet encoding, bool createIntermediateGroups)
    {
        return H5PAdapter.Create(H5P.LINK_CREATE, h =>
        {
            return new H5LinkCreationPropertyList(h)
            {
                CharacterEncoding = encoding,
                CreateIntermediateGroups = createIntermediateGroups
            };
        });
    }
}

