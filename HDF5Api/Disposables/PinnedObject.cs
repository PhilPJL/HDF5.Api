using System;

namespace HDF5Api.Disposables;

/// <summary>
///     Disposable wrapper for safely pinning/releasing objects
/// </summary>
public class PinnedObject : Disposable
{
    public GCHandle Pinned { get; private set; }

    public PinnedObject(object objectToPin)
    {
        Pinned = GCHandle.Alloc(objectToPin, GCHandleType.Pinned);
    }

    protected override void Dispose(bool _)
    {
        if (Pinned.IsAllocated)
        {
            Pinned.Free();
        }
    }

    public static unsafe implicit operator void*(PinnedObject pinned)
    {
        return pinned.Pinned.AddrOfPinnedObject().ToPointer();
    }

    public static implicit operator IntPtr(PinnedObject pinned)
    {
        return pinned.Pinned.AddrOfPinnedObject();
    }

    public static implicit operator GCHandle(PinnedObject pinned)
    {
        return pinned.Pinned;
    }
}
