using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Api
{
    public abstract class H5TypeAdapterBase
    {
        internal static readonly ASCIIEncoding Ascii = new();

        /// <summary>
        /// Helper method to copy fixed/max length string
        /// </summary>
        protected static unsafe void CopyString(string source, byte* destination, int destinationSizeInBytes)
        {
            if ((source ?? string.Empty) == string.Empty)
            {
                return;
            }

            byte[] sourceBytes = Ascii.GetBytes(source);

            var msg = $"The provided string has length {source?.Length} which exceeds the maximum destination length of {destinationSizeInBytes}.";

            if (source?.Length > destinationSizeInBytes)
            {
                throw new InvalidOperationException(msg);
            }

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(sourceBytes, 0).ToPointer(), destination, destinationSizeInBytes, source?.Length ?? 0);
        }

        /// <summary>
        /// Helper method to copy fixed/max length byte array.
        /// </summary>
        protected static unsafe void CopyBlob(byte[] source, byte* destination, int maxBlobSize, int blobTypeLength)
        {
            AssertBlobMaxLength(source, maxBlobSize);
            AssertBlobLengthDivisibleByTypeLength(source, blobTypeLength);

            if ((source?.Length ?? 0) > 0)
            {
                Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(source, 0).ToPointer(), destination, maxBlobSize, source?.Length ?? 0);
            }
        }

        public static void AssertBlobMaxLength(byte[] values, int maxBlobSize)
        {
            if ((values?.Length ?? 0) > maxBlobSize)
            {
                throw new InvalidOperationException($"The provided data blob is length {values?.Length} exceeds the maximum expected length {maxBlobSize}");
            }
        }

        public static void AssertBlobLengthDivisibleByTypeLength(byte[] values, int blobTypeLength)
        {
            var length = values?.Length ?? 0;
            AssertBlobLengthDivisibleByTypeLength(length, blobTypeLength);
        }

        public static void AssertBlobLengthDivisibleByTypeLength(int length, int blobTypeLength)
        {
            if (length % blobTypeLength != 0)
            {
                throw new InvalidOperationException($"The provided data blob is length {length} is not an exact multiple of contained type of length {blobTypeLength}");
            }
        }
    }
}
