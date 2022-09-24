using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

/*
 * File contains wrappers for all handle/H5ID types returned by the H5 PInvoke API.
 * These allow calling the correct Close method when disposed.
 */

namespace HDF5Api;

internal static class H5Handle
{
#if DEBUG
    public static ConcurrentDictionary<long, string> Handles { get; } = new();
#endif
    public static int OpenHandleCount { get; set; }

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
        Handles.Remove(handle, out var _);
#endif

        OpenHandleCount--;
    }
}

internal enum HandleType
{
    File = 1,
    Group = 2,
    Type = 3,
    Space = 4,
    DataSet = 5,
    Attribute = 6,

    PropertyList = 10,
}
