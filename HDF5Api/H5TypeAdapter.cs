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

        /// <summary>
        /// Helper method to copy fixed/max length byte array.
        /// </summary>
        protected static unsafe void CopyBlob(byte[] source, byte* destination, int maxBlobSize, int blobTypeLength)
        {
            AssertBlobMaxLength(source, maxBlobSize);
            AssertBlobLengthDivisibleByTypeLength(source, blobTypeLength);

            if ((source?.Length ?? 0) > 0)
            {
                Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(source, 0).ToPointer(), destination, maxBlobSize, source.Length);
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
            if ((values?.Length ?? 0) % blobTypeLength != 0)
            {
                throw new InvalidOperationException($"The provided data blob is length {values?.Length} is not an exact multiple of contained type of length {blobTypeLength}");
            }
        }
    }

    /// <summary>
    /// Base class for implementing a custom adaptor/converter to format an instance of C# type into a blittable struct for use in an HDF5 dataset
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

    /// <summary>
    /// TODO: implement a generic type adaptor that works from attributed properties in the target type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public class H5TypeAdapter<TInput> : IH5TypeAdapter<TInput>
    {
        private int TInputSize { get; set; }

        public H5Type GetH5Type()
        {
            // Generate H5Type.

            // Read properties using reflection - simple types, attributed strings(length) and attributed byte arrays(contained type)
            // Calculate size of TInput memory block required
            TInputSize = 28; // dummy

            // Complex types - for another day.

            throw new NotImplementedException();
        }

        public void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            if((inputRecords?.Count() ?? 0) == 0)
            {
                return;
            }

            // Allocate
            using var memory = new GlobalMemory(TInputSize * inputRecords.Count());

            // Copy/marshal individual properties into memory



            write(memory);

            throw new NotImplementedException();
        }

        public static H5TypeAdapter<TInput> Default { get; } = new H5TypeAdapter<TInput>();
    }

    internal class GlobalMemory : Disposable
    {
        public GlobalMemory(int size)
        {
            IntPtr = Marshal.AllocHGlobal(size);
        }

        public IntPtr IntPtr { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (IntPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(IntPtr);
                IntPtr = IntPtr.Zero;
            }
        }

        public static implicit operator IntPtr(GlobalMemory memory)
        {
            return memory.IntPtr;
        }
    }
}
