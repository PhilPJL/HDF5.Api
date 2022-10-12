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

namespace HDF5.Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5I
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Retrieves a name of an object based on the object identifier.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetName
    /// </summary>
    /// <param name="obj_id">Identifier of the object. This identifier can
    /// refer to a group, dataset, or named datatype.</param>
    /// <param name="name">A name associated with the identifier.</param>
    /// <param name="size">The size of the name buffer; must be the size of
    /// the name in bytes plus 1 for a <code>NULL</code> terminator.</param>
    /// <returns>Returns the length of the name if successful, returning 0
    /// (zero) if no name is associated with the identifier. Otherwise 
    /// returns a negative value.</returns>
    /// <remarks>ASCII strings ONLY! This function does not work with UTF-8
    /// encoded strings. See JIRA issue HDF5/HDFFV-9686.</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Iget_name"), SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial ssize_t get_name
        (hid_t obj_id, Span<byte> name, nint size);
#endif
}
