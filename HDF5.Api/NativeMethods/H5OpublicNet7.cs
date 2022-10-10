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
internal static partial class H5O
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Retrieves the metadata for an object specified by an identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5O.html#Object-GetInfo
    /// </summary>
    /// <param name="loc_id">Identifier for object of type specified by
    /// <code>H5O.type_t</code></param>
    /// <param name="oinfo">Buffer in which to return object information</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_info1",
        CallingConvention = CallingConvention.Cdecl),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    public static extern herr_t get_info(hid_t loc_id, ref info_t oinfo);

    /// <summary>
    /// Retrieves the metadata for an object, identifying the object by
    /// location and relative name.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5O.html#Object-GetInfoByName
    /// </summary>
    /// <param name="loc_id">File or group identifier specifying location
    /// of group in which object is located</param>
    /// <param name="name">Name of group, relative to
    /// <paramref name="loc_id"/></param>
    /// <param name="oinfo">Buffer in which to return object information</param>
    /// <param name="lapl_id">Link access property list</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY!</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Oget_info_by_name1", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller)),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t get_info_by_name
        (hid_t loc_id, string name, ref info_t oinfo,
        hid_t lapl_id = H5P.DEFAULT);
#endif
}
