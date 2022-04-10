using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Api
{
    public abstract class H5TypeAdapterBase
    {
        protected static readonly ASCIIEncoding Ascii = new();

        /// <summary>
        /// Helper method to copy fixed/max length string
        /// </summary>
        protected static unsafe void CopyString(string source, byte* destination, int destinationSizeInBytes)
        {
            // TODO: Move to 'unsafe' helper class?
            // probably useful outside of H5TypeAdapter
            byte[] sourceBytes = Ascii.GetBytes(source);

            var msg = $"Profile: The provided string '{source}' has length {source.Length} which exceeds the maximum destination length of {destinationSizeInBytes}.";

            if (source.Length > destinationSizeInBytes)
            {
                throw new InvalidOperationException(msg);
            }

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(sourceBytes, 0).ToPointer(), destination, destinationSizeInBytes, source.Length);
        }

        protected static unsafe void CopyBlob(byte[] source, byte* destination, int maxBlobSize, int blobTypeLength)
        {
            AssertBlobMaxLength(source, maxBlobSize);
            AssertBlobLengthDivisibleByTypeLength(source, blobTypeLength);

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(source, 0).ToPointer(), destination, maxBlobSize, source.Length);
        }

        public static void AssertBlobMaxLength(byte[] values, int maxBlobSize)
        {
            if (values.Length > maxBlobSize)
            {
                throw new InvalidOperationException($"The provided data blob is length {values.Length} exceeds the maximum expected length {maxBlobSize}");
            }
        }

        public static void AssertBlobLengthDivisibleByTypeLength(byte[] values, int blobTypeLength)
        {
            if (values.Length % blobTypeLength != 0)
            {
                throw new InvalidOperationException($"The provided data blob is length {values.Length} is not an exact multiple of contained type of length {blobTypeLength}");
            }
        }
    }

    /// <summary>
    /// Base class for implementing an adaptor/converter to format an instance of C# type into a blittable struct for use in an HDF5 dataset
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class H5TypeAdapter<TInput, TOutput> : H5TypeAdapterBase, IH5TypeAdapter<TInput>
    {
        protected abstract TOutput Convert(TInput source);

        public abstract H5Type GetH5Type();

        public void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            var records = inputRecords.Select(Convert).ToArray();

            GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

            try
            {
                write(pinnedBuffer.AddrOfPinnedObject());
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
    }
}
