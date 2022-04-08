using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Api
{
    public interface IH5DataSetWriter<in CompoundType> : IDisposable where CompoundType : struct
    {
        public void Write(IEnumerable<CompoundType> recordsChunk);
        public int CurrentPosition { get; }
    }

    public static class H5DataSetWriter
    {
        public static readonly ulong[] MaxDims = new ulong[] { H5S.UNLIMITED };

        public static IH5DataSetWriter<CompoundType> CreateOneDimensionalDataSetWriter<CompoundType>(IH5Location location, string dataSetName, Func<H5Type> typeFactory, int chunkSize = 100) where CompoundType : struct
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

            var h5CompoundType = typeFactory();

            // Create a dataset with our record type and chunk size.
            // TODO: get h5CompoundType from CompoundType and own h5CompoundType - get rid of typeFactory?
            var dataSet = location.CreateDataSet(dataSetName, h5CompoundType, memorySpace, properyList);

            // Writer owns and disposes/releases the data-set.
            return new H5DataSetWriter1D<CompoundType>(dataSet, h5CompoundType, true);
        }
    }

    public class H5DataSetWriter1D<CompoundType> : Disposable, IH5DataSetWriter<CompoundType> where CompoundType : struct
    {
        internal H5DataSetWriter1D(H5DataSet h5DataSet, H5Type h5Type, bool ownsDataSet = false)
        {
            DataSet = h5DataSet;
            Type = h5Type;
            OwnsDataSet = ownsDataSet;
        }

        private H5DataSet DataSet { get; set; }
        private H5Type Type { get; set; }
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

        public void Write(IEnumerable<CompoundType> recordsChunk)
        {
            var records = recordsChunk.ToArray();

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
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
    }
}
