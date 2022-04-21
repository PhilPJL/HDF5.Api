using HDF5Api.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5Api
{
    /// <summary>
    /// Base class for implementing a custom adaptor/converter to format an instance of a C# type into a blittable struct for use in an HDF5 dataset
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public abstract class H5TypeAdapter<TInput> : H5TypeAdapterBase, IH5TypeAdapter<TInput>
    {
        public abstract H5Type GetH5Type();

        public abstract void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords);
    }

    /// <summary>
    /// Base class for implementing a custom adaptor/converter to format an instance of C# type into a blittable struct for use in an HDF5 dataset
    /// </summary>
    /// <remarks>
    /// It is required to implement the <see cref="Convert(TInput)"/> method to convert <typeparamref name="TInput"/> to <typeparamref name="TOutput"/>, and
    /// to implement the <see cref="H5TypeAdapter{TInput}.GetH5Type"/> method to provide a matching H5 type definition. 
    /// </remarks>
    public abstract class H5TypeAdapter<TInput, TOutput> : H5TypeAdapter<TInput>
    {
        protected abstract TOutput Convert(TInput source);

        public override void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            // convert input to Array of struct
            var records = inputRecords.Select(Convert).ToArray();
            // pin
            using (var pinnedRecords = new PinnedObject(records))
            {
                // write to HDF5
                write(pinnedRecords);
            }
        }
    }

#if false
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

        public override void Write(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            if ((inputRecords?.Count() ?? 0) == 0)
            {
                return;
            }

            // Allocate
            using (var memory = new GlobalMemory(TInputSize * inputRecords.Count()))
            {
                // TODO: Copy/marshal individual properties into memory

                write(memory);
            }

            throw new NotImplementedException();
        }

        public static H5AutoTypeAdapter<TInput> Default { get; } = new H5AutoTypeAdapter<TInput>();
    }
#endif
}
