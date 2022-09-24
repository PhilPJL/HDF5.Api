using System.Runtime.CompilerServices;

namespace HDF5Api;

public static class H5ThrowExtensions
{
    public static void ThrowIfInvalidHandleValue(this long handle, [CallerMemberName] string? methodName = null)
    {
        if (handle <= H5Handle.InvalidHandleValue)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Hdf5Exception($"Bad handle {handle}.");
            }
            else
            {
                throw new Hdf5Exception($"Bad handle {handle} when {methodName}.");
            }
        }
    }

    public static void ThrowIfDefaultOrInvalidHandleValue(this long handle)
    {
        if (handle <= H5Handle.DefaultHandleValue)
        {
            throw new Hdf5Exception($"Bad handle {handle}.");
        }
    }

    public static void ThrowIfError(this int err, string methodName)
    {
        if (err < 0)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Hdf5Exception($"Error: {err}.");
            }
            else
            {
                throw new Hdf5Exception($"Error {err} calling: {methodName}.");
            }
        }
    }

    internal static void AssertIsHandleType(this long handle, params HandleType[] types)
    {
        handle.ThrowIfInvalidHandleValue();

        var type = handle >> 56;

        foreach (var t in types)
        {
            if ((int)t == type) { return; }
        }

        throw new Hdf5Exception($"Handle type {type} is not valid at this point.");
    }
}
