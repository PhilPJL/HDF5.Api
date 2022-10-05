using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace HDF5Api;

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

internal enum HandleType
{
    File = 1,
    Group = 2,
    Type = 3,
    Space = 4,
    DataSet = 5,
    Attribute = 6,

    PropertyList = 10
}
