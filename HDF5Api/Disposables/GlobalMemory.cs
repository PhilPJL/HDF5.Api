namespace HDF5Api.Disposables;

#if NETSTANDARD
/// <summary>
///     Disposable wrapper for safe allocation of global memory
/// </summary>
internal abstract class GlobalMemoryBase : Disposable
{
    public IntPtr IntPtr { get; protected set; }

    protected override void Dispose(bool _)
    {
        if (IntPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(IntPtr);
            IntPtr = IntPtr.Zero;
        }
    }

    public static unsafe implicit operator void*(GlobalMemoryBase memory)
    {
        return memory.IntPtr.ToPointer();
    }

    public static implicit operator IntPtr(GlobalMemoryBase memory)
    {
        return memory.IntPtr;
    }
}

internal class GlobalMemory : GlobalMemoryBase
{
    public GlobalMemory(int size)
    {
        IntPtr = Marshal.AllocHGlobal(size);
    }
}

internal class StringToGlobalMemoryAnsi : GlobalMemoryBase
{
    public StringToGlobalMemoryAnsi(string s)
    {
        IntPtr = Marshal.StringToHGlobalAnsi(s);
    }
}
#endif