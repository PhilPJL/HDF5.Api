using System;

namespace HDF5Api;

/// <summary>
///     Base class for H5 object types.
/// </summary>
/// <typeparam name="THandle">The handle associated with this object type.</typeparam>
public class H5Object<T> : Disposable where T : H5Object<T>
{
    #region Constructor and operators

    public long Handle { get; private set; } = H5Handle.DefaultHandleValue;
    private readonly Action<T>? _closeHandle;

    internal H5Object(long handle, Action<T>? closeHandle)
    {
        handle.ThrowIfDefaultOrInvalidHandleValue();

        Handle = handle;
        _closeHandle = closeHandle;

        if (_closeHandle != null)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public bool IsDisposed()
    {
        return Handle == H5Handle.InvalidHandleValue;
    }

    protected override void Dispose(bool _)
    {
        if (_closeHandle == null || Handle == H5Handle.DefaultHandleValue)
        {
            // native or default(0) handle shouldn't be disposed
            return;
        }

        if (Handle == H5Handle.InvalidHandleValue)
        {
            // already disposed
            throw new ObjectDisposedException($"{GetType().FullName}");
        }

        // close and mark as disposed
        _closeHandle((T)this);
        H5Handle.UntrackHandle(Handle);

        // disposed
        Handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5Object<T>? h5object)
    {
        if (h5object == null) { return 0; }

        h5object.Handle.ThrowIfInvalidHandleValue();
        return h5object.Handle;
    }

    #endregion
}



