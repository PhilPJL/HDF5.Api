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
internal static partial class H5S
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Releases and terminates access to a dataspace.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-Close
    /// </summary>
    /// <param name="space_id">Identifier of dataspace to release.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sclose"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t close(hid_t space_id);

    /// <summary>
    /// Creates a new dataspace of a specified type.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-Create
    /// </summary>
    /// <param name="type">Type of dataspace to be created.</param>
    /// <returns>Returns a dataspace identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Screate"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create(class_t type);

    /// <summary>
    /// Creates a new simple dataspace and opens it for access.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-CreateSimple
    /// </summary>
    /// <param name="rank">Number of dimensions of dataspace.</param>
    /// <param name="dims">Array specifying the size of each dimension.</param>
    /// <param name="maxdims">Array specifying the maximum size of each
    /// dimension.</param>
    /// <returns>Returns a dataspace identifier if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Screate_simple"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hid_t create_simple
        (int rank,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] hsize_t[] dims,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] hsize_t[] maxdims);

    /// <summary>
    /// Retrieves dataspace dimension size and maximum size.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-ExtentDims
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace object to query</param>
    /// <param name="dims">Pointer to array to store the size of each dimension.</param>
    /// <param name="maxdims">Pointer to array to store the maximum size of each dimension.</param>
    /// <returns>Returns the number of dimensions in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    /// <remarks>Either or both of <paramref name="dims"/> and
    /// <paramref name="maxdims"/> may be <code>NULL</code>.</remarks>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_dims"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial int get_simple_extent_dims
        (hid_t space_id, Span<hsize_t> dims, Span<hsize_t> maxdims);

    /// <summary>
    /// Determines the dimensionality of a dataspace.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-ExtentNdims
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace</param>
    /// <returns>Returns the number of dimensions in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_ndims"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial int get_simple_extent_ndims(hid_t space_id);

    /// <summary>
    /// Determines the number of elements in a dataspace.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-ExtentNpoints
    /// </summary>
    /// <param name="space_id">Identifier of the dataspace object to query</param>
    /// <returns>Returns the number of elements in the dataspace if
    /// successful; otherwise returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sget_simple_extent_npoints"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial hssize_t get_simple_extent_npoints(hid_t space_id);

    /// <summary>
    /// Selects a hyperslab region to add to the current selected region.
    /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5S.html#Dataspace-SelectHyperslab
    /// </summary>
    /// <param name="space_id">Identifier of dataspace selection to modify</param>
    /// <param name="op">Operation to perform on current selection.</param>
    /// <param name="start">Offset of start of hyperslab</param>
    /// <param name="stride">Number of blocks included in hyperslab.</param>
    /// <param name="count">Hyperslab stride.</param>
    /// <param name="block">Size of block in hyperslab.</param>
    /// <returns>Returns a non-negative value if successful; otherwise
    /// returns a negative value.</returns>
    [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_hyperslab"),
    SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    public static partial herr_t select_hyperslab
        (hid_t space_id, seloper_t op,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[] start,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[] stride,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[] count,
        [MarshalAs(UnmanagedType.LPArray)] hsize_t[] block);
#endif
}