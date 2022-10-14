#if NETSTANDARD
namespace HDF5.Api.Disposables;

/// <summary>
///     Disposable wrapper for safe allocation of global memory
/// </summary>
internal class GlobalMemory : Disposable
{
    public GlobalMemory(int size)
    {
        IntPtr = Marshal.AllocHGlobal(size);
    }

    public IntPtr IntPtr { get; private set; }

    protected override void Dispose(bool _)
    {
        if (IntPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(IntPtr);
            IntPtr = IntPtr.Zero;
        }
    }

    public static unsafe implicit operator void*(GlobalMemory memory)
    {
        return memory.IntPtr.ToPointer();
    }

    public static implicit operator IntPtr(GlobalMemory memory)
    {
        return memory.IntPtr;
    }
}

#endif