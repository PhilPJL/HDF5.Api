using System;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5G (Group) API.
/// </summary>
public struct H5Group : IDisposable
{
    #region Handle wrappers
    private long Handle { get; set; } = H5Handle.InvalidHandleValue;

    private H5Group(Handle handle)
    {
        Handle = handle;
        H5Handle.TrackHandle(handle);
    }

    public void Dispose()
    {
        if (Handle != H5Handle.InvalidHandleValue)
        {
            NativeMethods.Close(this);
            H5Handle.UntrackHandle(Handle);
            Handle = H5Handle.InvalidHandleValue;
        }
    }

    public static implicit operator long(H5Group h5object)
    {
        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }
    #endregion

    internal static partial class NativeMethods
    {
        #region Close

        //[LibraryImport(Constants.DLLFileName, EntryPoint = "H5Gclose"), SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        //[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        //private static partial int Close(long handle);

        public static void Close(H5Group attribute)
        {
            //int err = Close(attribute.Handle);
            int err = H5G.close(attribute.Handle);
            // TODO: get additional error info 
            err.ThrowIfError("H5Gclose");
        }

        #endregion

        #region Create

        public static H5Group Create(H5Location locationId, string name)
        {
            locationId.ThrowIfNotValid();

            Handle h = H5G.create(locationId.Handle, name);

            h.ThrowIfInvalidHandleValue("H5G.create");

            return new H5Group(h);
        }

        #endregion
    }

    #region C level API wrappers


    public static H5Group Open(H5Location locationId, string name)
    {
        locationId.ThrowIfNotValid();

        Handle h = H5G.open(locationId.Handle, name);

        h.ThrowIfInvalidHandleValue("H5G.open");

        return new H5Group(h);
    }

    public static void Delete(H5Location locationId, string path)
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
    public static bool Exists(H5Location locationId, string name)
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
    public static bool PathExists(H5Location locationId, string path)
    {
        locationId.ThrowIfNotValid();
        var ginfo = new H5G.info_t();
        int err = H5G.get_info_by_name(locationId, path, ref ginfo);
        return err >= 0;
    }

    #endregion
}
