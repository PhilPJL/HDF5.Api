namespace HDF5.Api;

internal static class H5ThrowHelpers
{
    public static void ThrowIfInvalidHandleValue(this long handle, [CallerMemberName] string? methodName = null)
    {
        if (handle <= H5Handle.InvalidHandleValue)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Hdf5Exception($"Bad handle [{handle}].");
            }

            throw new Hdf5Exception($"Bad handle [{handle}] in method: {methodName}.");
        }
    }

    public static void ThrowIfDefaultOrInvalidHandleValue(this long handle, string message)
    {
        if (handle <= H5Handle.DefaultHandleValue)
        {
            throw new Hdf5Exception($"Bad handle [{handle}] when: {message}.");
        }
    }

    public static void ThrowIfError(this int err, [CallerMemberName] string? methodName = null)
    {
        if (err < 0)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Hdf5Exception($"Error: {err}.");
            }

            throw new Hdf5Exception($"Error {err} calling: {methodName}.");
        }
    }

    public static void ThrowIfError(this long err, [CallerMemberName] string? methodName = null)
    {
        if (err < 0)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Hdf5Exception($"Error: {err}.");
            }

            throw new Hdf5Exception($"Error {err} calling: {methodName}.");
        }
    }

    public static void ThrowOnAttributeStorageMismatch<T>(int attributeStorageSize, int marshalSize)
    {
        if (marshalSize != attributeStorageSize)
        {
            throw new Hdf5Exception(
              $"Attribute storage size is {attributeStorageSize}, which does not match the marshalable size for type {typeof(T).Name} of {marshalSize}.");
        }
    }
}
