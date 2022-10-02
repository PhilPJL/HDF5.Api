namespace HDF5Api.Disposables;

/// <summary>
///     Disposable wrapper for safe allocation of global memory
/// </summary>
/// <remarks>
///     TODO: investigate various SafeHandle types available in .NET
/// </remarks>
public abstract class GlobalMemoryBase : Disposable
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

public class GlobalMemory : GlobalMemoryBase
{
    public GlobalMemory(int size)
    {
        IntPtr = Marshal.AllocHGlobal(size);
    }
}

public class StringToGlobalMemoryAnsi : GlobalMemoryBase
{
    public StringToGlobalMemoryAnsi(string s)
    {
        IntPtr = Marshal.StringToHGlobalAnsi(s);
    }
}
