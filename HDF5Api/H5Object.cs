using System;

namespace HDF5Api;

/// <summary>
///     Base class for H5 object types.
/// </summary>
/// <typeparam name="THandle">The handle associated with this object type.</typeparam>
/*public class H5Object<THandle> : Disposable where THandle : IH5Object
{
    public H5Object(THandle handle)
    {
        Handle = handle;
    }

    public THandle Handle { get; }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !Handle.IsDisposed)
        {
            Handle.Dispose();
        }
    }

    public static implicit operator THandle(H5Object<THandle> h5Object)
    {
        return h5Object.Handle;
    }
}
*/


