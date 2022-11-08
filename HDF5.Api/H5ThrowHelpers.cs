using HDF5.Api.Utils;
using System.Diagnostics;

namespace HDF5.Api;

internal static class H5ThrowHelpers
{
    public static long ThrowIfInvalidHandleValue(this long handle, [CallerMemberName] string? methodName = null)
    {
        if (handle <= H5Handle.InvalidHandleValue)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new H5Exception($"Bad handle [{handle}].");
            }

            throw new H5Exception($"Bad handle [{handle}] in method: {methodName}.");
        }

        return handle;
    }

    public static long ThrowIfDefaultOrInvalidHandleValue(this long handle, string message)
    {
        if (handle <= H5Handle.DefaultHandleValue)
        {
            throw new H5Exception($"Bad handle [{handle}] when: {message}.");
        }

        return handle;
    }

    public static int ThrowIfError(this int result, [CallerMemberName] string? methodName = null)
    {
        if (result < 0)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new H5Exception($"Error: {result}.");
            }

            throw new H5Exception($"Error {result} calling: {methodName}.");
        }

        return result;
    }

    public static long ThrowIfError(this long result, [CallerMemberName] string? methodName = null)
    {
        if (result < 0)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new H5Exception($"Error: {result}.");
            }

            throw new H5Exception($"Error {result} calling: {methodName}.");
        }

        return result;
    }

    public static void ThrowOnAttributeStorageMismatch<T>(int attributeStorageSize, int marshalSize)
    {
        if (marshalSize != attributeStorageSize)
        {
            throw new H5Exception(
              $"Attribute storage size is {attributeStorageSize}, which does not match the marshalable size for type {typeof(T).Name} of {marshalSize}.");
        }
    }

    internal static void ThrowIfNotEnum<T>()
    {
        if (!typeof(T).IsEnum)
        {
            throw new InvalidOperationException($"The generic typep parameter 'T' is of type {typeof(T).Name}.  An enum type was expected.");
        }
    }

    internal static void ThrowIfManaged<T>()
    {
        if (!typeof(T).IsUnmanaged())
        {
            throw new InvalidOperationException($"{typeof(T).Name} is a managed type. Only unmanaged types are valid.");
        }
    }
}
