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

internal static partial class H5E
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Turns automatic error printing on or off.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5E.html#Error-SetAuto2
    /// </summary>
    /// <param name="estack_id">Error stack identifier.</param>
    /// <param name="func">Function to be called upon an error condition.</param>
    /// <param name="client_data">Data passed to the error function.</param>
    /// <returns>Returns a non-negative value on success; otherwise returns
    /// a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Eset_auto2"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t set_auto
        (hid_t estack_id, auto_t func, IntPtr client_data);
#endif
}