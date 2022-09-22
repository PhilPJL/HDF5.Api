using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HDF5Api;

public static class H5ThrowExtensions
{
/*    public static void ThrowIfNotValid(this H5Handle handle, [CallerMemberName] string methodName = null)
    {
        ThrowIfNotValid(handle.Handle, methodName ?? handle.GetType().Name);
    }

    public static void ThrowIfNotValid(this IH5Object handle, [CallerMemberName] string methodName = null)
    {
        ThrowIfNotValid(handle.Handle, methodName ?? handle.GetType().Name);
    }

    public static void ThrowIfNotValid(this H5Object<H5Type> handle, [CallerMemberName] string methodName = null)
    {
        ThrowIfNotValid(handle.Handle, methodName ?? handle.GetType().Name);
    }
*/
    public static void ThrowIfInvalidHandleValue(this long handle, [CallerMemberName] string methodName = null)
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

    public static void ThrowIfDefaultOrInvalidHandleValue(this long handle, [CallerMemberName] string methodName = null)
    {
        if (handle <= H5Handle.DefaultHandleValue)
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

        if (!types.Any(t => (int)t == type))
        {
            throw new Hdf5Exception($"Handle type {type} is not valid at this point.");
        }
    }
}
