using System;

namespace HDF5Api;

/// <summary>
///     Wrapper for H5S (Space) API.
/// </summary>
public struct H5Space : IDisposable
{
    #region Constructor and operators

    private long Handle { get; set; } = H5Handle.DefaultHandleValue;
    private readonly bool _isNative = false;

    internal H5Space(long handle, bool isNative = false)
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
        H5SpaceNativeMethods.Close(this);
        H5Handle.UntrackHandle(Handle);
        Handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5Space h5object)
    {
        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }

    #endregion

    #region Public Api

    public void SelectHyperslab(int offset, int count)
    {
        H5SpaceNativeMethods.SelectHyperslab(this, offset, count);
    }

    public long GetSimpleExtentNPoints()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNPoints(this);
    }

    public int GetSimpleExtentNDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentNDims(this);
    }

    public (int rank, ulong[] dims, ulong[] maxDims) GetSimpleExtentDims()
    {
        return H5SpaceNativeMethods.GetSimpleExtentDims(this);
    }

    #endregion
}
