using System.Linq;

namespace HDF5.Api;

/// <summary>
///     Base class for H5 object types.
/// </summary>
/// <typeparam name="T">The H5 type derived from this H5Object.</typeparam>
public class H5Object<T> : Disposable where T : H5Object<T>
{
    #region Constructor and operators

    private long _handle;

    private readonly Action<T>? _closeHandle;

    internal H5Object(long handle, HandleType handleType, Action<T>? closeHandle)
    {
        handle.ThrowIfDefaultOrInvalidHandleValue($"Constructing {handleType}");

#if DEBUG
        AssertHasHandleType(handle, handleType);
#endif

        _handle = handle;
        _closeHandle = closeHandle;

        if (_closeHandle != null)
        {
            H5Handle.TrackHandle(handle);
        }
    }

    public bool IsDisposed => _handle == H5Handle.InvalidHandleValue;

    /// <summary>
    /// Disposes unmanaged handle by calling appropriate 'H5X.close' method.
    /// </summary>
    /// <param name="_">We don't have any unmanaged resources so ignored 'disposing' param.</param>
    protected override void Dispose(bool _)
    {
        if (_closeHandle == null || _handle == H5Handle.DefaultHandleValue)
        {
            // native or default(0) handle shouldn't be disposed
            return;
        }

        if (IsDisposed)
        {
            // Dispose is idempotent (so don't throw if already disposed).
            return;
        }

        // close and mark as disposed
        _closeHandle((T)this);
        H5Handle.UntrackHandle(_handle);

        // disposed
        _handle = H5Handle.InvalidHandleValue;
    }

    public static implicit operator long(H5Object<T>? h5Object)
    {
        if (h5Object == null)
        {
            // To allow passing null as default value which then gets converted to 0.
            return H5Handle.DefaultHandleValue;
        }

        h5Object.AssertNotDisposed();
        h5Object._handle.ThrowIfInvalidHandleValue();

        return h5Object._handle;
    }

    private static void AssertHasHandleType(long handle, params HandleType[] types)
    {
        handle.ThrowIfInvalidHandleValue();

        var type = handle & (long)HandleType.Mask;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var t in types)
        {
            if ((long)t == type) { return; }
        }

        throw new H5Exception($"Handle type '{(HandleType)type}' is not valid.  Expecting one of: {string.Join(", ", types.OrderBy(s => s))}.");
    }

    internal void AssertHasHandleType(params HandleType[] types)
    {
        AssertNotDisposed();
        AssertHasHandleType(_handle, types);
    }

    internal void AssertHasLocationHandleType() => AssertHasHandleType(HandleType.File, HandleType.Group);
    internal void AssertHasWithAttributesHandleType() => AssertHasHandleType(HandleType.File, HandleType.Group, HandleType.DataSet, HandleType.Type);

    internal void AssertNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    #endregion
}