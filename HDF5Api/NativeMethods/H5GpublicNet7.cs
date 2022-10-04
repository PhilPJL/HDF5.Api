/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace HDF5Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5G
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Closes the specified group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5G.html#Group-Close
    /// </summary>
    /// <param name="group_id">Group identifier to release.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t group_id);

    /// <summary>
    /// Creates a new group and links it into the file.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5G.html#Group-Create2
    /// </summary>
    /// <param name="loc_id">File or group identifier</param>
    /// <param name="name">Absolute or relative name of the link to the
    /// new group</param>
    /// <param name="lcpl_id">Link creation property list identifier</param>
    /// <param name="gcpl_id">Group creation property list identifier</param>
    /// <param name="gapl_id">Group access property list identifier</param>
    /// <returns>Returns a group identifier if successful; otherwise returns
    /// a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gcreate2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create
        (hid_t loc_id, string name, hid_t lcpl_id = H5P.DEFAULT,
        hid_t gcpl_id = H5P.DEFAULT, hid_t gapl_id = H5P.DEFAULT);

    /// <summary>
    /// Gets a group creation property list identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5G.html#Group-GetCreatePlist
    /// </summary>
    /// <param name="group_id"> Identifier of the group.</param>
    /// <returns>Returns an identifier for the group’s creation property
    /// list if successful. Otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gget_create_plist"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t get_create_plist(hid_t group_id);

    /// <summary>
    /// Retrieves information about a group.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5G.html#Group-GetInfoByName
    /// </summary>
    /// <param name="loc_id">File or group identifier</param>
    /// <param name="name">Name of group for which information is to be
    /// retrieved</param>
    /// <param name="ginfo">Struct in which group information is returned</param>
    /// <param name="lapl_id">Link access property list</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strngs ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gget_info_by_name", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_info_by_name
        (hid_t loc_id, string name, ref info_t ginfo,
        hid_t lapl_id = H5P.DEFAULT);

    /// <summary>
    /// Opens an existing group with a group access property list.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5G.html#Group-Open2
    /// </summary>
    /// <param name="loc_id">File or group identifier specifying the
    /// location of the group to be opened</param>
    /// <param name="name">Name of the group to open</param>
    /// <param name="gapl_id">Group access property list identifier</param>
    /// <returns>Returns a group identifier if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gopen2", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t open
        (hid_t loc_id, string name, hid_t gapl_id = H5P.DEFAULT);
#endif
}
