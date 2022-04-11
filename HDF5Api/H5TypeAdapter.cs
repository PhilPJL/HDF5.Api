using HDF5Api.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    /// <summary>
    /// Base class for implementing a custom adaptor/converter to format an instance of a C# type into a blittable struct for use in an HDF5 dataset
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public abstract class H5TypeAdapter<TInput> : H5TypeAdapterBase, IH5TypeAdapter<TInput>
    {
        public abstract H5Type GetH5Type();

        public abstract void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
    }

    /// <summary>
    /// Base class for implementing a custom adaptor/converter to format an instance of C# type into a blittable struct for use in an HDF5 dataset
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class H5TypeAdapter<TInput, TOutput> : H5TypeAdapter<TInput>
    {
        protected abstract TOutput Convert(TInput source);

        public override void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
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
    public class H5AutoTypeAdapter<TInput> : H5TypeAdapter<TInput>
    {
        private int TInputSize { get; set; }

        public override H5Type GetH5Type()
        {
            // Generate H5Type.

            // Read properties using reflection - simple types, attributed strings(length) and attributed byte arrays(contained type)
            // Calculate size of TInput memory block required
            TInputSize = 28; // dummy

            // Complex types - for another day.

            throw new NotImplementedException();
        }

        public override void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            if ((inputRecords?.Count() ?? 0) == 0)
            {
                return;
            }

            // Allocate
            using var memory = new GlobalMemory(TInputSize * inputRecords.Count());

            // Copy/marshal individual properties into memory



            write(memory);

            throw new NotImplementedException();
        }

        public static H5AutoTypeAdapter<TInput> Default { get; } = new H5AutoTypeAdapter<TInput>();
    }
}
