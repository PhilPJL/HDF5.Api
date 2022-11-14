using HDF5.Api.NativeMethods;
using System.Collections.Generic;
using System.Linq;
using static HDF5.Api.NativeMethods.H5L;
using HDF5.Api.Utils;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_l.html"/>
/// </summary>
internal static unsafe class H5LAdapter
{
    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList)
        where T : H5Location<T>
    {
        location.AssertHasLocationHandleType();

#if NET7_0_OR_GREATER
        return exists(location, name, linkAccessPropertyList).ThrowIfError() > 0;
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            return exists(location, nameBytesPtr, linkAccessPropertyList).ThrowIfError() > 0;
        }
#endif
    }

    internal static void Delete<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList)
        where T : H5Location<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet, HandleType.Attribute);

#if NET7_0_OR_GREATER
        delete(location, name, linkAccessPropertyList).ThrowIfError();
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            delete(location, nameBytesPtr, linkAccessPropertyList).ThrowIfError();
        }
#endif
    }

    private static IEnumerable<(string name, ObjectType type)> GetMembers<T>(H5Location<T> location, H5O.type_t type)
        where T : H5Location<T>
    {
        ulong idx = 0;

        var names = new List<(string, ObjectType)>();

        iterate(location, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero).ThrowIfError();

        return names;

        int Callback(long locationId, IntPtr intPtrName, ref info_t info, IntPtr _)
        {
            try
            {
                var name = info.cset switch
                {
                    H5T.cset_t.ASCII or H5T.cset_t.UTF8 => MarshalHelpers.PtrToStringUTF8(intPtrName),
                    // Don't throw inside callback - see HDF docs
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(name))
                {
                    var oinfo = H5OAdapter.GetInfoByName(locationId, name);

                    if (oinfo.type == type || type == H5O.type_t.UNKNOWN)
                    {
                        names.Add((name, (ObjectType)oinfo.type));
                    }
                }

                return 0;
            }
            catch
            {
                // Don't throw inside callback - see HDF docs
                return -1;
            }
        }
    }

    internal static IEnumerable<string> GetGroupNames<T>(H5Location<T> location)
        where T : H5Location<T> => GetMembers(location, H5O.type_t.GROUP).Select(m => m.name);

    internal static IEnumerable<string> GetDataSetNames<T>(H5Location<T> location)
        where T : H5Location<T> => GetMembers(location, H5O.type_t.DATASET).Select(m => m.name);

    internal static IEnumerable<string> GetNamedDataTypeNames<T>(H5Location<T> location)
        where T : H5Location<T> => GetMembers(location, H5O.type_t.NAMED_DATATYPE).Select(m => m.name);

    internal static IEnumerable<(string name, ObjectType type)> GetMembers<T>(H5Location<T> location)
        where T : H5Location<T> => GetMembers(location, H5O.type_t.UNKNOWN);

    /// <summary>
    /// Create a new link creation property list
    /// </summary>
    /// <param name="encoding"></param>
    /// <param name="createIntermediateGroups"></param>
    /// <returns></returns>
    internal static H5LinkCreationPropertyList CreateCreationPropertyList(CharacterSet encoding, bool createIntermediateGroups)
    {
        return H5PAdapter.Create(H5P.LINK_CREATE, h => new H5LinkCreationPropertyList(h)
        {
            CharacterEncoding = encoding,
            CreateIntermediateGroups = createIntermediateGroups
        });
    }
}

