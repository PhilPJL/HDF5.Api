
using HDF5Api.NativeMethods;

namespace HDF5Api;

/// <summary>
/// H5 group native methods: <see href="https://docs.hdfgroup.org/hdf5/v1_10/group___h5_g.html"/>
/// </summary>
internal static partial class H5GroupNativeMethods
{
    #region Close

    /// <summary>
    /// Closes the specified group.
    /// </summary>
    /// <param name="group_id">Group identifier to release.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gclose")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Gclose(hid_t group_id);

    public static void Close(H5Group attribute)
    {
        int err = H5Gclose(attribute);

        err.ThrowIfError(nameof(H5Gclose));
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates a new group and links it into the file.
    /// </summary>
    /// <param name="loc_id">File or group identifier</param>
    /// <param name="name">Absolute or relative name of the link to the
    /// new group</param>
    /// <param name="lcpl_id">Link creation property list identifier</param>
    /// <param name="gcpl_id">Group creation property list identifier</param>
    /// <param name="gapl_id">Group access property list identifier</param>
    /// <returns>Returns a group identifier if successful; otherwise returns
    /// a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gcreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Gcreate2(hid_t loc_id, string name, hid_t lcpl_id, hid_t gcpl_id, hid_t gapl_id);

    public static H5Group Create<T>(
        H5Location<T> location, string name,
        H5PropertyList? propListLinkCreation = null,
        H5PropertyList? propListGroupCreation = null,
        H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Gcreate2(location, name, propListLinkCreation, propListGroupCreation, propListGroupAccess);

        h.ThrowIfInvalidHandleValue(nameof(H5Gcreate2));

        return new H5Group(h);
    }

    #endregion

    #region Open

    /// <summary>
    /// Opens an existing group with a group access property list.
    /// </summary>
    /// <param name="loc_id">File or group identifier specifying the
    /// location of the group to be opened</param>
    /// <param name="name">Name of the group to open</param>
    /// <param name="gapl_id">Group access property list identifier</param>
    /// <returns>Returns a group identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gopen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial hid_t H5Gopen2(hid_t loc_id, string name, hid_t gapl_id);

    public static H5Group Open<T>(H5Location<T> location, string name, H5PropertyList? propListGroupAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        long h = H5Gopen2(location, name, propListGroupAccess);

        h.ThrowIfInvalidHandleValue(nameof(H5Gopen2));

        return new H5Group(h);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Removes a link from a group.
    /// </summary>
    /// <param name="loc_id">Identifier of the file or group containing
    /// the object.</param>
    /// <param name="name">Name of the link to delete.</param>
    /// <param name="lapl_id">Link access property list identifier.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ldelete", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Ldelete(hid_t loc_id, string name, hid_t lapl_id);

    public static void Delete<T>(H5Location<T> location, string path, H5PropertyList? propListLinkAccess = null) where T : H5Object<T>
    {
        location.AssertHasHandleType(HandleType.File, HandleType.Group);

        int err = H5Ldelete(location, path, propListLinkAccess);

        err.ThrowIfError(nameof(H5Ldelete));
    }

    #endregion

    #region Exists

    /// <summary>
    /// Determine whether a link with the specified name exists in a group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5L.html#Link-Exists
    /// </summary>
    /// <param name="loc_id">Identifier of the file or group to query.</param>
    /// <param name="name">The name of the link to check.</param>
    /// <param name="lapl_id">Link access property list identifier.</param>
    /// <returns>Returns 1 or 0 if successful; otherwise returns a negative
    /// value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lexists", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial htri_t H5Lexists(hid_t loc_id, string name, hid_t lapl_id);

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
        int err = H5Lexists(location, name, linkAccessPropertyList);

        err.ThrowIfError(nameof(H5Lexists));

        return err > 0;

        static bool IsSimpleName(string name)
        {
            return !name.Contains('\\') && !name.Contains('/');
        }
    }

    #endregion

    #region Path exists

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
        int err = H5Gget_info_by_name(location, path, ref ginfo, linkAccessPropertyList);
        return err >= 0;
    }

    /// <summary>
    /// Retrieves information about a group.
    /// </summary>
    /// <param name="loc_id">File or group identifier</param>
    /// <param name="name">Name of group for which information is to be
    /// retrieved</param>
    /// <param name="ginfo">Struct in which group information is returned</param>
    /// <param name="lapl_id">Link access property list</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strngs ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gget_info_by_name", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial herr_t H5Gget_info_by_name(hid_t loc_id, string name, ref info_t ginfo, hid_t lapl_id);

    private enum storage_type_t
    {
        UNKNOWN = -1,
        SYMBOL_TABLE,
        COMPACT,
        DENSE
    }

    private struct info_t
    {
        public storage_type_t storage_type;

        public ulong nlinks;

        public long max_corder;

        public uint mounted;
    }

    #endregion
}
