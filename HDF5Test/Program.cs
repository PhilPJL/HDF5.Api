using HDF.PInvoke;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            CreateFile();
            CreateFile();
        }

        static void CreateFile()
        {
            const long invalidHandle = -1L;
            var rawRecordTypeId = invalidHandle;

            try
            {
                Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");

                // Create file
                using var fileId = H5File.Create(@"test1.h5", H5F.ACC_TRUNC);
                Console.WriteLine($"Created file: {fileId}");

                // setup compound type
                int size = Marshal.SizeOf<RawRecord>();
                Console.WriteLine($"Datatype size = {size}");

                rawRecordTypeId = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
                Console.WriteLine($"Created type: {rawRecordTypeId}");

                H5T.insert(rawRecordTypeId, "Id", Marshal.OffsetOf<RawRecord>("Id"), H5T.NATIVE_INT64);
                H5T.insert(rawRecordTypeId, "Measurement Id", Marshal.OffsetOf<RawRecord>("MeasurementId"), H5T.NATIVE_INT32);
                H5T.insert(rawRecordTypeId, "Timestamp", Marshal.OffsetOf<RawRecord>("Timestamp"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Thickness", Marshal.OffsetOf<RawRecord>("Thickness"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Profile deviation", Marshal.OffsetOf<RawRecord>("ProfileDeviation"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Profile height", Marshal.OffsetOf<RawRecord>("ProfileHeight"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Z position", Marshal.OffsetOf<RawRecord>("ZPosition"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Interval Id", Marshal.OffsetOf<RawRecord>("IntervalId"), H5T.NATIVE_INT64);
                H5T.insert(rawRecordTypeId, "Pulse offset", Marshal.OffsetOf<RawRecord>("PulseOffset"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "Reference offset", Marshal.OffsetOf<RawRecord>("ReferenceOffset"), H5T.NATIVE_INT64);

                var records = GetTestData();

                // create a dataspace - single dimension 1 x unlimited
                var dims = new ulong[] { (ulong)records.Length };
                var maxdims = new ulong[] { H5S.UNLIMITED };

                using var spaceId = H5Space.CreateSimple(1, dims, maxdims);
                Console.WriteLine($"Created space: {spaceId}");

                // create a group
                using var groupId = H5Group.Create(fileId, "Data");
                Console.WriteLine($"Created group: {groupId}");

                // create a dataset-create property list
                using var propListId = H5PropertyList.Create(H5P.DATASET_CREATE);
                Console.WriteLine($"Created prop: {propListId}");
                // 1) allow chunking - doesn't work without this. From user guide: HDF5 requires the use of chunking when defining extendable datasets
                H5PropertyList.SetChunk(propListId, 1, dims);
                // 2) enable compression
                H5PropertyList.EnableCompression(propListId, 6);

                // create the dataset RawRecords 
                using var dataSetId = H5DataSet.Create(groupId, "RawRecords", rawRecordTypeId, spaceId, propListId);
                Console.WriteLine($"Created data set: {dataSetId}");

                for (int i = 0; i < 1; i++)
                {
                    GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

                    try
                    {
                        H5D.set_extent(dataSetId, dims);
                        H5D.write(dataSetId, rawRecordTypeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinnedBuffer.AddrOfPinnedObject());
                        dims[0] += (ulong)records.Length;
                    }
                    finally
                    {
                        pinnedBuffer.Free();
                    }
                }
            }
            finally
            {
                int result;

                if (rawRecordTypeId > -1)
                {
                    // close compound type
                    result = H5T.close(rawRecordTypeId);

                    Console.WriteLine($"Closed data type: {rawRecordTypeId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5T.close={result}.");
                    }
                }
            }
        }

        static RawRecord[] GetTestData()
        {
            var now = DateTime.UtcNow;

            return Enumerable.Range(0, 100000)
                .Select(i => new RawRecord
                {
                    Id = i,
                    ProfileDeviation = 5.5 + i / 1000f,
                    Timestamp = now.AddMilliseconds(i).ToOADate(),
                    Thickness = 0.2 + i / 1000f

                })
                .ToArray();
        }
    }

    //CTSWaveformAndProfileDatabaseSpectra
    [StructLayout(LayoutKind.Sequential)]
    struct RawRecord
    {
        public long Id;
        public int MeasurementId;
        public double Timestamp;
        public double Thickness;
        public double ProfileDeviation;
        public double ProfileHeight;
        public double ZPosition;
        public long IntervalId;
        public double PulseOffset;
        public long ReferenceOffset;
    }
}
