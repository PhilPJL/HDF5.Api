using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Api
{
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

//#if DEBUG
//        private int TickCount;
//#endif

        public void Write(IEnumerable<TInput> recordsChunk)
        {
//#if DEBUG
//            TickCount = TickCount == 0 ? Environment.TickCount : TickCount;
//            Console.WriteLine($"Writing {recordsChunk.Count()} {typeof(TInput).Name}, {Environment.TickCount - TickCount}");
//            TickCount = Environment.TickCount;
//#endif
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

//#if DEBUG
//                Console.WriteLine($"Written {recordsChunk.Count()} {typeof(TInput).Name}, {Environment.TickCount - TickCount}");
//#endif
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
    }
}
