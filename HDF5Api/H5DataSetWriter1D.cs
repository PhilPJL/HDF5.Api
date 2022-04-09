using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Api
{
    public abstract class H5TypeConverterBase
    {
        protected static readonly ASCIIEncoding Ascii = new();
    }

    public interface IH5TypeConverter<in TInput, out TOutput> where TOutput : struct
    {
        public H5Type CreateH5Type();
        public TOutput Convert(TInput source);
    }

    public interface IH5DataSetWriter<in TInput, out TOutput> : IDisposable where TOutput : struct
    {
        public void Write(IEnumerable<TInput> recordsChunk);
        public int CurrentPosition { get; }
    }

    public static class H5DataSetWriter
    {
        public static readonly ulong[] MaxDims = new ulong[] { H5S.UNLIMITED };

        public static IH5DataSetWriter<TInput, TOutput> CreateOneDimensionalDataSetWriter<TInput, TOutput>(IH5Location location, string dataSetName, IH5TypeConverter<TInput, TOutput> converter, int chunkSize = 100) where TOutput : struct
        {
            // NOTE: we're only interested in creating a data set currently, not opening an existing one

            // Single dimension (rank 1), unlimited length, chunk size.
            using var memorySpace = H5Space.CreateSimple(1, new ulong[] { (ulong)chunkSize }, MaxDims);
            Console.WriteLine($"Created space: {memorySpace}");

            // Create a dataset-creation property list
            using var properyList = H5PropertyList.Create(H5P.DATASET_CREATE);

            // Enable chunking. From the user guide: "HDF5 requires the use of chunking when defining extendable datasets."
            properyList.SetChunk(1, new ulong[] { (ulong)chunkSize });

            // TODO: investigate performance of compression and different compression types
            //properyList.EnableDeflateCompression(6);

            var h5CompoundType = converter.CreateH5Type();

            // Create a dataset with our record type and chunk size.
            // TODO: get h5CompoundType from CompoundType and own h5CompoundType - get rid of typeFactory?
            var dataSet = location.CreateDataSet(dataSetName, h5CompoundType, memorySpace, properyList);

            // Writer owns and disposes/releases the data-set.
            return new H5DataSetWriter1D<TInput, TOutput>(dataSet, h5CompoundType, converter.Convert, true);
        }
    }

    public class H5DataSetWriter1D<TInput, TOutput> : Disposable, IH5DataSetWriter<TInput, TOutput> where TOutput : struct
    {
        internal H5DataSetWriter1D(H5DataSet h5DataSet, H5Type h5Type, Func<TInput, TOutput> convert, bool ownsDataSet = false)
        {
            DataSet = h5DataSet;
            Type = h5Type;
            Convert = convert;
            OwnsDataSet = ownsDataSet;
        }

        private H5DataSet DataSet { get; set; }
        private H5Type Type { get; set; }
        private Func<TInput, TOutput> Convert { get; }
        private bool OwnsDataSet { get; }
        public int CurrentPosition { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // We may own the data-set
                if (OwnsDataSet)
                {
                    DataSet?.Dispose();
                    DataSet = null;
                }

                // We do own the type
                Type?.Dispose();
                Type = null;
            }
        }

        private int TickCount;

        public void Write(IEnumerable<TInput> recordsChunk)
        {
            TickCount = TickCount == 0 ? Environment.TickCount : TickCount;
            Console.WriteLine($"Writing {recordsChunk.Count()} {typeof(TInput).Name}, {Environment.TickCount - TickCount}");
            TickCount = Environment.TickCount;

            var records = recordsChunk.Select(Convert).ToArray();

            GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

            try
            {
                // Extend the dataset to accept this chunk
                DataSet.SetExtent(new ulong[] { (ulong)(CurrentPosition + records.Length) });

                // Move the hyperslab window
                using var fileSpace = DataSet.GetSpace();
                fileSpace.SelectHyperslab(CurrentPosition, records.Length);

                CurrentPosition += records.Length;

                // Match the space to length of records retrieved.
                using var recordSpace = H5Space.CreateSimple(1, new ulong[] { (ulong)records.Length }, H5DataSetWriter.MaxDims);
                DataSet.Write(Type, recordSpace, fileSpace, pinnedBuffer.AddrOfPinnedObject());
                Console.WriteLine($"Written {recordsChunk.Count()} {typeof(TInput).Name}, {Environment.TickCount - TickCount}");
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
    }
}
