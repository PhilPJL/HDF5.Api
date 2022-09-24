namespace HDF5Api;

internal static partial class H5GroupNativeMethods
{
    #region Close

    //[LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gclose"), SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    //[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    //private static partial int Close(long handle);

    public static void Close(H5Group attribute)
    {
        //int err = Close(attribute.Handle);
        int err = H5G.close(attribute);
        // TODO: get additional error info 
        err.ThrowIfError("H5Gclose");
    }

    #endregion

    #region Create

    public static H5Group Create(long locationId, string name,
        H5PropertyList? propListLinkCreation = null,
        H5PropertyList? propListGroupCreation = null,
        H5PropertyList? propListGroupAccess = null)
    {
        locationId.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5G.create(locationId, name, propListLinkCreation, propListGroupCreation, propListGroupAccess);

        h.ThrowIfInvalidHandleValue("H5G.create");

        return new H5Group(h);
    }

    #endregion

    public static H5Group Open(long locationId, string name, H5PropertyList? propListGroupAccess = null)
    {
        locationId.AssertIsHandleType(HandleType.File, HandleType.Group);

        long h = H5G.open(locationId, name, propListGroupAccess);

        h.ThrowIfInvalidHandleValue("H5G.open");

        return new H5Group(h);
    }

    public static void Delete(long locationId, string path, H5PropertyList? propListLinkAccess = null)
    {
        locationId.AssertIsHandleType(HandleType.File, HandleType.Group);

        int err = H5L.delete(locationId, path, propListLinkAccess);

        err.ThrowIfError("H5L.delete");
    }

    /// <summary>
    ///     Test if an object exists by name in the specified location.
    /// </summary>
    /// <param name="locationId">A file or group id</param>
    /// <param name="name">A simple object name, e.g. 'group' not 'group/sub-group'.</param>
    public static bool Exists(long locationId, string name)
    {
        locationId.AssertIsHandleType(HandleType.File, HandleType.Group);

        if (!IsSimpleName(name))
        {
            throw new Hdf5Exception($"Only simple group names are allowed, not '{name}'.");
        }

        // NOTE: H5L.exists can only check for a direct child of locationId
        int err = H5L.exists(locationId, name);

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
    public static bool PathExists(long locationId, string path)
    {
        locationId.AssertIsHandleType(HandleType.File, HandleType.Group);

        var ginfo = new H5G.info_t();
        int err = H5G.get_info_by_name(locationId, path, ref ginfo);
        return err >= 0;
    }
}
