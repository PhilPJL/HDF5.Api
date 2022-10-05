using HDF5Api.NativeMethods;
using static HDF5Api.NativeMethods.H5G;

namespace HDF5Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_g.html"/>
/// </summary>
internal static class H5GAdapter
{
    public static void Close(H5Group attribute)
    {
        int err = close(attribute);

        err.ThrowIfError(nameof(close));
    }

    public static H5Group Create<T>(
        H5Location<T> location, string name,
        H5PropertyList? propListLinkCreation = null,
        H5PropertyList? propListGroupCreation = null,
        H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = create(location, name, propListLinkCreation, propListGroupCreation, propListGroupAccess);

        h.ThrowIfInvalidHandleValue(nameof(create));

        return new H5Group(h);
    }

    public static H5Group Open<T>(H5Location<T> location, string name, H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = open(location, name, propListGroupAccess);

        h.ThrowIfInvalidHandleValue(nameof(open));

        return new H5Group(h);
    }

    public static void Delete<T>(H5Location<T> location, string path, H5PropertyList? propListLinkAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        H5LAdapter.Delete(location, path, propListLinkAccess);
    }

    /// <summary>
    ///     Test if an object exists by name in the specified location.
    /// </summary>
    /// <param name="location">A file or group id</param>
    /// <param name="name">A simple object name, e.g. 'group' not 'group/sub-group'.</param>
    /// <param name="linkAccessPropertyList"></param>
    public static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        if (!IsSimpleName(name))
        {
            throw new Hdf5Exception($"Only simple group names are allowed, not '{name}'.");
        }

        // NOTE: H5L.exists can only check for a direct child of locationId
        int err = H5L.exists(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5L.exists));

        return err > 0;

        static bool IsSimpleName(string name)
        {
#if NETSTANDARD
            return !name.Contains("\\") && !name.Contains("/");
#else
            return !name.Contains('\\') && !name.Contains('/');
#endif
        }
    }

    /// <summary>
    ///     This version of Exists can accept a path
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Note that if the path doesn't exist then by default HDF5 logs an error to the console
    ///         which can cause performance issues.  It's recommended to use <see cref="H5Error.SetAutoOff" />
    ///         to turn off logging in performance is critical.
    ///     </para>
    ///     <para>
    ///         Note also that an H5 file allows a rooted path (/grp/grp) but an H5 group doesn't (grp/grp).
    ///     </para>
    /// </remarks>
    /// <param name="location">A file or group id</param>
    /// <param name="path">e.g. /group/sub-group/sub-sub-group</param>
    /// <param name="linkAccessPropertyList"></param>
    public static bool PathExists<T>(H5Location<T> location, string path, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        info_t ginfo = default;
        int err = get_info_by_name(location, path, ref ginfo, linkAccessPropertyList);
        return err >= 0;
    }

    public static H5PropertyList CreatePropertyList(PropertyList list)
    {
        return list switch
        {
            PropertyList.Create => H5PAdapter.Create(H5P.GROUP_CREATE),
            PropertyList.Access => H5PAdapter.Create(H5P.GROUP_ACCESS),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static H5PropertyList GetPropertyList(H5Group group, PropertyList list)
    {
        return list switch
        {
            PropertyList.Create => H5PAdapter.GetPropertyList(group, get_create_plist),
            _ => throw new NotImplementedException(),
        };
    }
}