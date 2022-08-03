namespace HDF5Api;

/// <summary>
///     Wrapper for H5G (Group) API.
/// </summary>
public class H5Group : H5Location<H5GroupHandle>
{
    private H5Group(Handle handle) : base(new H5GroupHandle(handle)) { }

    #region C level API wrappers

    public static H5Group Create(H5LocationHandle locationId, string name)
    {
        locationId.ThrowIfNotValid();

        Handle h = H5G.create(locationId.Handle, name);

        h.ThrowIfNotValid("H5G.create");

        return new H5Group(h);
    }

    public static H5Group Open(H5LocationHandle locationId, string name)
    {
        locationId.ThrowIfNotValid();

        Handle h = H5G.open(locationId.Handle, name);

        h.ThrowIfNotValid("H5G.open");

        return new H5Group(h);
    }

    public static void Delete(H5LocationHandle locationId, string path)
    {
        locationId.ThrowIfNotValid();

        int err = H5L.delete(locationId.Handle, path);

        err.ThrowIfError("H5L.delete");
    }

    /// <summary>
    ///     Test if an object exists by name in the specified location.
    /// </summary>
    /// <param name="locationId">A file or group id</param>
    /// <param name="name">A simple object name, e.g. 'group' not 'group/sub-group'.</param>
    public static bool Exists(H5LocationHandle locationId, string name)
    {
        locationId.ThrowIfNotValid();

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
            return !name.Contains("\\") && !name.Contains("/");
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
    public static bool PathExists(H5LocationHandle locationId, string path)
    {
        locationId.ThrowIfNotValid();
        var ginfo = new H5G.info_t();
        int err = H5G.get_info_by_name(locationId, path, ref ginfo);
        return err >= 0;
    }

    #endregion
}
