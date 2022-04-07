using HDF.PInvoke;
using HDF5Api;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test
{
    internal static class CreateFileTest
    {
        // Working demo of creating chunked dataset with compound type.
        internal unsafe static void CreateFile()
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");

            // Create file
            using var file = H5File.Create(@"test-dummydata.h5", H5F.ACC_TRUNC);
            Console.WriteLine($"Created file: {file}");

            // setup compound type
            // How to have variable length arrays per run?
            int size = Marshal.SizeOf<RawRecord>();
            Console.WriteLine($"Datatype size = {size}, {size - RawRecord.waveformBlobSize - RawRecord.profileBlobSize}");

            using var rawRecordType = H5Type.CreateCompoundType(size);
            Console.WriteLine($"Created type: {rawRecordType}");

            rawRecordType.Insert("Id", Marshal.OffsetOf<RawRecord>("Id"), H5T.NATIVE_INT64);
            rawRecordType.Insert("Measurement Id", Marshal.OffsetOf<RawRecord>("MeasurementId"), H5T.NATIVE_INT32);
            rawRecordType.Insert("Timestamp", Marshal.OffsetOf<RawRecord>("Timestamp"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Thickness", Marshal.OffsetOf<RawRecord>("Thickness"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Profile deviation", Marshal.OffsetOf<RawRecord>("ProfileDeviation"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Profile height", Marshal.OffsetOf<RawRecord>("ProfileHeight"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Z position", Marshal.OffsetOf<RawRecord>("ZPosition"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Interval Id", Marshal.OffsetOf<RawRecord>("IntervalId"), H5T.NATIVE_INT64);
            rawRecordType.Insert("Pulse offset", Marshal.OffsetOf<RawRecord>("PulseOffset"), H5T.NATIVE_DOUBLE);
            rawRecordType.Insert("Reference offset", Marshal.OffsetOf<RawRecord>("ReferenceOffset"), H5T.NATIVE_DOUBLE);

            using var waveformArrayType = H5Type.CreateDoubleArrayType(RawRecord.waveformBlobSize / sizeof(double));
            rawRecordType.Insert("Waveform", Marshal.OffsetOf<RawRecord>("Waveform"), waveformArrayType);
            using var profileArrayType1 = H5Type.CreateDoubleArrayType(RawRecord.profileBlobSize / sizeof(double) / 2);
            rawRecordType.Insert("ProfileX", Marshal.OffsetOf<RawRecord>("Profile1"), profileArrayType1);
            using var profileArrayType2 = H5Type.CreateDoubleArrayType(RawRecord.profileBlobSize / sizeof(double) / 2);
            rawRecordType.Insert("ProfileZ", Marshal.OffsetOf<RawRecord>("Profile2"), profileArrayType2);

            int chunkSize = 100;

            // create a dataspace - single dimension 1 x unlimited
            // this is the chunk size that the dataset extends itself by
            var dims = new ulong[] { (ulong)chunkSize };
            var maxdims = new ulong[] { H5S.UNLIMITED };

            // a dataspace defining the chunk size of our data set
            // Q: why do we need a memory space with chunk size, and a property list with the same chunk size - or do we?
            using var memorySpace = H5Space.CreateSimple(1, dims, maxdims);
            Console.WriteLine($"Created space: {memorySpace}");

            // create a dataset-create property list
            using var properyList = H5PropertyList.Create(H5P.DATASET_CREATE);
            Console.WriteLine($"Created prop: {properyList}");
            // 1) allow chunking - doesn't work without this. From user guide: HDF5 requires the use of chunking when defining extendable datasets
            properyList.SetChunk(1, dims);
            // 2) enable compression
            //properyList.EnableDeflateCompression(6);

            // create a group name 'Data'
            using var group = file.CreateGroup("Data");
            Console.WriteLine($"Created group: {group}");

            // create a dataset named 'RawRecords' in group 'Data' with our record type and chunk size
            using var dataSet = group.CreateDataSet("RawRecords", rawRecordType, memorySpace, properyList);
            Console.WriteLine($"Created data set: {dataSet}");

            Stopwatch s = new Stopwatch();
            s.Start();

            var extent = new ulong[] { 0 };

            for (int i = 0; i < 50; i++)
            {
                var records = GetTestData(i, 100);

                //Console.WriteLine(records.Length);

                GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

                try
                {
                    // record current position for the hyperslab window
                    int currentPosition = (int)extent[0];

                    // extend the dataset to accept this chunk
                    extent[0] = (ulong)(currentPosition + records.Length);
                    dataSet.SetExtent(extent);

                    // move the hyperslab window
                    using var fileSpace = dataSet.GetSpace();
                    fileSpace.SelectHyperslab(currentPosition, records.Length);

                    // match the space to length of records retrieved
                    // if using standard length chunks (say 100) then only need to change this for the final write
                    using var recordSpace = H5Space.CreateSimple(1, new ulong[] { (ulong)records.Length }, maxdims);
                    dataSet.Write(rawRecordType, recordSpace, fileSpace, pinnedBuffer.AddrOfPinnedObject());
                }
                finally
                {
                    pinnedBuffer.Free();
                }
            }

            s.Stop();
            Console.WriteLine($"Time elapsed: {s.Elapsed}. Total rows {extent[0]}.");
        }

        static RawRecord[] GetTestData(int n, int chunk)
        {
            var now = DateTime.UtcNow;

            return Enumerable.Range(n, chunk)
                .Select(i => new RawRecord
                {
                    Id = i,
                    ProfileDeviation = 5.5 + i / 1000f,
                    Timestamp = now.AddMilliseconds(i).ToOADate(),
                    Thickness = 0.2 + i / 1000f
                })
                .ToArray();
        }

        //CTSWaveformAndProfileDatabaseSpectra
        [StructLayout(LayoutKind.Sequential)]
        unsafe struct RawRecord
        {
            public const int waveformBlobSize = 16384;
            public const int profileBlobSize = 32768;

            public RawRecord()
            {
                Id = 0;
                MeasurementId = 0;
                Timestamp = 0;
                Thickness = 0;
                ProfileDeviation = 0;
                ProfileHeight = 0;
                ZPosition = 0;
                IntervalId = 0;
                PulseOffset = 0;
                ReferenceOffset = 0;
            }

            public long Id;
            public int MeasurementId;
            public double Timestamp;
            public double Thickness;
            public double ProfileDeviation;
            public double ProfileHeight;
            public double ZPosition;
            public long IntervalId;
            public double PulseOffset;
            public double ReferenceOffset;
            public fixed byte Waveform[waveformBlobSize];
            public fixed byte Profile1[profileBlobSize / 2];
            public fixed byte Profile2[profileBlobSize / 2];
        }
    }
}

