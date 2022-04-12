using System;
using System.Collections.Generic;
using System.Linq;

namespace HDF5Api
{
    /// <summary>
    /// Implementation of a chunked, extensible, 1D data-set writer
    /// </summary>
    /// <remarks>
    /// Writes a 1D array of TInput instance to the supplied data-set.  Requires an appropriate adapter to convert TInput to a suitable form.
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
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
        public int RowsWritten { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Finalize the length of the data set
                DataSet.SetExtent(new ulong[] { (ulong)RowsWritten });

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

        /// <summary>
        /// Write a collection of <typeparamref name="TInput"/> to the DataSet.
        /// </summary>
        public void WriteChunk(IEnumerable<TInput> recordsChunk)
        {
            int numRecords = recordsChunk.Count();

            // Extend the dataset to accept this chunk
            DataSet.SetExtent(new ulong[] { (ulong)(RowsWritten + numRecords) });

            // Move the hyperslab window
            using var fileSpace = DataSet.GetSpace();
            fileSpace.SelectHyperslab(RowsWritten, numRecords);

            // Match the space to length of records retrieved.
            using var recordSpace = H5Space.CreateSimple(1, new ulong[] { (ulong)numRecords }, H5DataSetWriter.MaxDims1D);

            // Configure most parameters for DataSet.WriteChunk and then pass the curried method as an Action<IntPtr> to Converter which only needs to supply the last param.
            Converter.WriteChunk(WriteAdaptor(DataSet, Type, recordSpace, fileSpace), recordsChunk);

            RowsWritten += numRecords;

            static Action<IntPtr> WriteAdaptor(H5DataSet dataSet, H5Type type, H5Space recordSpace, H5Space fileSpace)
            {
                return (IntPtr buffer) => dataSet.Write(type, recordSpace, fileSpace, buffer);
            }
        }
    }
}
