using System;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5P (Property list) API.
/// </summary>
public struct H5PropertyList : IDisposable
{
    #region Constructor and operators

    private long Handle { get; set; } = H5Handle.DefaultHandleValue;
    private readonly bool _isNative = false;

    internal H5PropertyList(long handle, bool isNative = false)
    {
        handle.ThrowIfDefaultOrInvalidHandleValue();

        Handle = handle;
        _isNative = isNative;

        if (!_isNative)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public void Dispose()
    {
        if (_isNative || Handle == H5Handle.DefaultHandleValue)
        {
            // native or default(0) handle shouldn't be disposed
            return;
        }

        if (Handle == H5Handle.InvalidHandleValue)
        {
            // already disposed
            // TODO: throw already disposed
        }

        // close and mark as disposed
        H5PropertyListNativeMethods.Close(this);
        H5Handle.UntrackHandle(Handle);
        Handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5PropertyList h5Object)
    {
        h5Object.Handle.ThrowIfInvalidHandleValue();
        return h5Object.Handle;
    }

    #endregion

    public void SetChunk(int rank, ulong[] dims)
    {
        H5PropertyListNativeMethods.SetChunk(this, rank, dims);
    }

    /// <summary>
    ///     Level 0 = off
    ///     Level 1 = min compression + min CPU
    ///     ..
    ///     Level 9 = max compression + max CPU and time
    /// </summary>
    /// <param name="level"></param>
    public void EnableDeflateCompression(uint level)
    {
        H5PropertyListNativeMethods.EnableDeflateCompression(this, level);
    }
}

