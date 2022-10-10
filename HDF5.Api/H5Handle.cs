using System.Collections.Concurrent;
using System.Diagnostics;

namespace HDF5.Api;

public static class H5Handle
{
#if DEBUG
    private static ConcurrentDictionary<long, string> Handles { get; } = new();
#endif
    public static int OpenHandleCount { get; private set; }

    internal const long InvalidHandleValue = -1;
    internal const long DefaultHandleValue = 0;

    internal static void TrackHandle(long handle)
    {
        handle.ThrowIfInvalidHandleValue();

#if DEBUG
        Handles.TryAdd(handle, Environment.StackTrace);
#endif

        OpenHandleCount++;
    }

    internal static void UntrackHandle(long handle)
    {
#if DEBUG
        Handles.TryRemove(handle, out _);
#endif

        OpenHandleCount--;
    }

#if DEBUG
    public static void DumpOpenHandles()
    {
        foreach (var kvp in Handles)
        {
            Debug.WriteLine($"Handle open{Environment.NewLine}{kvp.Value}");
        }
    }
#endif
}

internal enum HandleType : long
{
    File = 1L << 56,
    Group = 2L << 56,
    Type = 3L << 56,
    Space = 4L << 56,
    DataSet = 5L << 56,
    Attribute = 6L << 56,

    PropertyList = 10L << 56,

    Mask = 0x7fL << 56
}
