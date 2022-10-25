using HDF5.Api.NativeMethods;
using static HDF5.Api.NativeMethods.H5G;

namespace HDF5.Api.NativeMethodAdapters;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_g.html"/>
/// </summary>
internal static unsafe class H5GAdapter
{
    internal static void Close(H5Group attribute)
    {
        close(attribute).ThrowIfError();
    }

    // Overload used by default which enabled intermediate group creation and UTF8 names.
    internal static H5Group Create<T>(
        H5Location<T> location, string name,
        H5GroupCreationPropertyList? propListGroupCreation) where T : H5Object<T>
    {
        using var propListLinkCreation = H5Link.CreateCreationPropertyList();

        return Create(location, name, propListLinkCreation, propListGroupCreation);
    }

    // Overload for test purposes
    internal static H5Group Create<T>(
        H5Location<T> location, string name,
        H5LinkCreationPropertyList? propListLinkCreation,
        H5GroupCreationPropertyList? propListGroupCreation) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        long h;

#if NET7_0_OR_GREATER
        h = create(location, name, propListLinkCreation, propListGroupCreation);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = create(location, nameBytesPtr, propListLinkCreation, propListGroupCreation);
        }
#endif

        h.ThrowIfInvalidHandleValue();

        return new H5Group(h);
    }

    internal static H5Group Open<T>(H5Location<T> location, string name, H5PropertyList? propListGroupAccess) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        long h;

#if NET7_0_OR_GREATER
        h = open(location, name, propListGroupAccess);
#else
        fixed (byte* nameBytesPtr = Encoding.UTF8.GetBytes(name))
        {
            h = open(location, nameBytesPtr, propListGroupAccess);
        }
#endif

        h.ThrowIfInvalidHandleValue();

        return new H5Group(h);
    }

    /// <summary>
    ///     Test if an object exists by name in the specified location.
    /// </summary>
    /// <param name="location">A file or group id</param>
    /// <param name="name">A simple object name, e.g. 'group' not 'group/sub-group'.</param>
    /// <param name="linkAccessPropertyList"></param>
    internal static bool Exists<T>(H5Location<T> location, string name, H5PropertyList? linkAccessPropertyList) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        if (!IsSimpleName(name))
        {
            throw new H5Exception($"Only simple group names are allowed, not '{name}'.");
        }

        // H5L.exists can only check for a direct child of locationId
        return H5LAdapter.Exists(location, name, linkAccessPropertyList);

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
    ///         which can cause performance issues.  It's recommended to use <see cref="H5Error.DisableErrorPrinting" />
    ///         to turn off logging in performance is critical.
    ///     </para>
    ///     <para>
    ///         Note also that an H5 file allows a rooted path (/grp/grp) but an H5 group doesn't (grp/grp).
    ///     </para>
    /// </remarks>
    /// <param name="location">A file or group id</param>
    /// <param name="path">e.g. /group/sub-group/sub-sub-group</param>
    /// <param name="linkAccessPropertyList"></param>
    internal static bool PathExists<T>(H5Location<T> location, string path, H5PropertyList? linkAccessPropertyList = null) where T : H5Object<T>
    {
        location.AssertHasLocationHandleType();

        info_t ginfo = default;
        int result;

#if NET7_0_OR_GREATER
        result = get_info_by_name(location, path, ref ginfo, linkAccessPropertyList);
#else
        fixed (byte* pathBytesPtr = Encoding.UTF8.GetBytes(path))
        {
            result = get_info_by_name(location, pathBytesPtr, ref ginfo, linkAccessPropertyList);
        }
#endif

        // NOTE: we don't throw on error here since we're relying 
        // on result <= 0 to mean 'path not found'.  
        // Not satisfying. 

        return result >= 0;
    }

    internal static H5GroupCreationPropertyList CreateCreationPropertyList()
    {
        return H5PAdapter.Create(H5P.GROUP_CREATE, h => new H5GroupCreationPropertyList(h));
    }

    /// <summary>
    /// Get copy of property list used to create the data-set.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    internal static H5GroupCreationPropertyList GetCreationPropertyList(H5Group group)
    {
        return H5PAdapter.GetPropertyList(group, get_create_plist, h => new H5GroupCreationPropertyList(h));
    }
}