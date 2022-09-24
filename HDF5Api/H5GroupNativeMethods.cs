using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF5Api;

internal static partial class H5GroupNativeMethods
{
    #region Close

    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gclose")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int H5Gclose(long handle);

    public static void Close(H5Group attribute)
    {
        int err = H5Gclose(attribute);

        err.ThrowIfError("H5Gclose");
    }

    #endregion

    #region Create

    public static H5Group Create<T>(H5Location<T> location, 
        string name,
        H5PropertyList? propListLinkCreation = null,
        H5PropertyList? propListGroupCreation = null,
        H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5G.create(location, name, propListLinkCreation, propListGroupCreation, propListGroupAccess);

        h.ThrowIfInvalidHandleValue("H5G.create");

        return new H5Group(h);
    }

    #endregion

    public static H5Group Open<T>(H5Location<T> location, string name, H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5G.open(location, name, propListGroupAccess);

        h.ThrowIfInvalidHandleValue("H5G.open");

        return new H5Group(h);
    }

    public static void Delete<T>(H5Location<T> location, string path, H5PropertyList? propListLinkAccess = null) where T : H5Object<T>
    {
        location.AssertIsHandleType(HandleType.File, HandleType.Group);

        int err = H5L.delete(location, path, propListLinkAccess);

        err.ThrowIfError("H5L.delete");
    }

    /// <summary>
    ///     Test if an object exists by name in the specified location.
    /// </summary>
    /// <param name="locationId">A file or group id</param>
    /// <param name="name">A simple object name, e.g. 'group' not 'group/sub-group'.</param>
    public static bool Exists<T>(H5Location<T> location, string name) where T : H5Object<T>
    {
        location.AssertIsHandleType(HandleType.File, HandleType.Group);

        if (!IsSimpleName(name))
        {
            throw new Hdf5Exception($"Only simple group names are allowed, not '{name}'.");
        }

        // NOTE: H5L.exists can only check for a direct child of locationId
        int err = H5L.exists(location, name);

        err.ThrowIfError("H5L.exists");

        return err > 0;

        static bool IsSimpleName(string name)
        {
            return !name.Contains('\\') && !name.Contains('/');
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
    /// <param name="locationId">A file or group id</param>
    /// <param name="path">e.g. /group/sub-group/sub-sub-group</param>
    public static bool PathExists<T>(H5Location<T> location, string path) where T : H5Object<T>
    {
        location.AssertIsHandleType(HandleType.File, HandleType.Group);

        var ginfo = new H5G.info_t();
        int err = H5G.get_info_by_name(location, path, ref ginfo);
        return err >= 0;
    }
}
