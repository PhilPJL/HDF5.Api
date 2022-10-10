namespace HDF5.Api;

public abstract class H5TypeAdapterBase
{
    internal static readonly ASCIIEncoding Ascii = new();
    internal static readonly UTF8Encoding UTF8 = new();

    /// <summary>
    ///     Helper method to copy fixed/max length string
    /// </summary>
    protected static unsafe void CopyString(string source, byte* destination, int destinationSizeInBytes)
    {
        if (string.IsNullOrEmpty(source))
        {
            return;
        }

        byte[] sourceBytes = Ascii.GetBytes(source);

        string msg =
            $"The provided string has length {source?.Length} which exceeds the maximum destination length of {destinationSizeInBytes}.";

        if (source?.Length > destinationSizeInBytes)
        {
            throw new InvalidOperationException(msg);
        }

        Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(sourceBytes, 0).ToPointer(), destination,
            destinationSizeInBytes, source?.Length ?? 0);
    }

    public static void AssertBlobLengthDivisibleByTypeLength(int length, int blobTypeLength)
    {
        if (length % blobTypeLength != 0)
        {
            throw new InvalidOperationException(
                $"The provided data blob is length {length} is not an exact multiple of contained type of length {blobTypeLength}");
        }
    }
}
