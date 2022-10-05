namespace HDF5Api;

/// <summary>
///     Base class for H5 object types.
/// </summary>
/// <typeparam name="T">The H5 type derived from this H5Object.</typeparam>
public class H5Object<T> : Disposable where T : H5Object<T>
{
    #region Constructor and operators

    private long _handle;

    private readonly Action<T>? _closeHandle;

    internal H5Object(long handle, Action<T>? closeHandle)
    {
        handle.ThrowIfDefaultOrInvalidHandleValue();

        _handle = handle;
        _closeHandle = closeHandle;

        if (_closeHandle != null)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public bool IsDisposed()
    {
        return _handle == H5Handle.InvalidHandleValue;
    }

    protected override void Dispose(bool _)
    {
        if (_closeHandle == null || _handle == H5Handle.DefaultHandleValue)
        {
            // native or default(0) handle shouldn't be disposed
            return;
        }

        if (_handle == H5Handle.InvalidHandleValue)
        {
            // already disposed
            throw new ObjectDisposedException($"{GetType().FullName}");
        }

        // close and mark as disposed
        _closeHandle((T)this);
        H5Handle.UntrackHandle(_handle);

        // disposed
        _handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5Object<T>? h5object)
    {
        if (h5object == null)
        {
            // To allow passing null as default value which then gets converted to 0.
            return H5Handle.DefaultHandleValue;
        }

        h5object._handle.ThrowIfInvalidHandleValue();
        return h5object._handle;
    }

    internal void AssertHasHandleType(params HandleType[] types)
    {
        _handle.ThrowIfInvalidHandleValue();

        var type = _handle >> 56;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var t in types)
        {
            if ((int)t == type) { return; }
        }

        throw new Hdf5Exception($"Handle type {type} is not valid at this point.");
    }

    #endregion
}