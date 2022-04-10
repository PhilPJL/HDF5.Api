using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5Api
{
    public class H5DataSetWriter1D<TInput> : Disposable, IH5DataSetWriter<TInput> 
    {
        internal H5DataSetWriter1D(H5DataSet h5DataSet, H5Type h5Type, IH5TypeAdapter<TInput> converter, bool ownsDataSet = false)
        {
            DataSet = h5DataSet;
            Type = h5Type;
            Converter = converter;
            OwnsDataSet = ownsDataSet;
        }

        private H5DataSet DataSet { get; set; }
        private H5Type Type { get; set; }
        private IH5TypeAdapter<TInput> Converter { get; }
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

        public void WriteChunk(IEnumerable<TInput> recordsChunk)
        {
            int numRecords = recordsChunk.Count();

            // Extend the dataset to accept this chunk
            DataSet.SetExtent(new ulong[] { (ulong)(CurrentPosition + numRecords) });

            // Move the hyperslab window
            using var fileSpace = DataSet.GetSpace();
            fileSpace.SelectHyperslab(CurrentPosition, numRecords);

            // Match the space to length of records retrieved.
            using var recordSpace = H5Space.CreateSimple(1, new ulong[] { (ulong)numRecords }, H5DataSetWriter.MaxDims);
            Converter.WriteChunk(WriteAdaptor(DataSet, Type, recordSpace, fileSpace), recordsChunk);

            CurrentPosition += numRecords;

            static Action<IntPtr> WriteAdaptor(H5DataSet dataSet, H5Type type, H5Space recordSpace, H5Space fileSpace)
            {
                return (IntPtr buffer) => dataSet.Write(type, recordSpace, fileSpace, buffer);
            }
        }
    }
}
