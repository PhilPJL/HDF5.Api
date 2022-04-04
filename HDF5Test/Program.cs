using HDF.PInvoke;
using System;
using System.Diagnostics;
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
            var fileId = invalidHandle;
            var rawRecordTypeId = invalidHandle;
            var spaceId = invalidHandle;
            var dataSetId = invalidHandle;
            var groupId = invalidHandle;
            var propListId = invalidHandle;

            try
            {
                // Create file
                fileId = H5F.create(@"test1.h5", H5F.ACC_TRUNC);
                Console.WriteLine($"Opened file: {fileId}");

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
                spaceId = H5S.create_simple(1, dims, new ulong[] { H5S.UNLIMITED });
                Console.WriteLine($"Created space: {spaceId}");

                // create a group
                groupId = H5G.create(fileId, "Data");
                Console.WriteLine($"Created group: {groupId}");

                // create a property list
                propListId = H5P.create(H5P.DATASET_CREATE);
                Console.WriteLine($"Created prop: {propListId}");
                // 1) allow chunking - doesn't work without this. From user guide: HDF5 requires the use of chunking when defining extendable datasets
                int err1 = H5P.set_chunk(propListId, 1, dims);
                if (err1 < 0)
                {
                    Console.WriteLine($"Error H5P.set_chunk={err1}.");
                }
                // 2) enable compression
                err1 = H5P.set_deflate(propListId, 6);
                if (err1 < 0)
                {
                    Console.WriteLine($"Error H5P.set_deflate={err1}.");
                }

                // create the dataset RawRecords 
                dataSetId = H5D.create(groupId, "RawRecords", rawRecordTypeId, spaceId, H5P.DEFAULT, propListId);
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
                // close file
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

                if (dataSetId > -1)
                {
                    // close data set
                    result = H5D.close(dataSetId);

                    Console.WriteLine($"Closed data set: {dataSetId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5D.close={result}.");
                    }
                }

                if (groupId > -1)
                {
                    // close data set
                    result = H5G.close(groupId);

                    Console.WriteLine($"Closed group: {groupId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5G.close={result}.");
                    }
                }

                if (spaceId > -1)
                {
                    // close space
                    result = H5S.close(spaceId);

                    Console.WriteLine($"Closed space: {spaceId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5S.close={result}.");
                    }
                }

                if (propListId > -1)
                {
                    // close space
                    result = H5P.close(propListId);

                    Console.WriteLine($"Closed prop: {propListId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5S.close={result}.");
                    }
                }

                Debug.WriteLine($"Closing file {fileId}.  Open object count: {H5F.get_obj_count(fileId, H5F.OBJ_ALL).ToInt32()}");

                if (fileId > -1)
                {
                    result = H5F.close(fileId);

                    Console.WriteLine($"Closed file: {fileId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5F.close={result}.");
                    }
                }
            }
        }

        static RawRecord[] GetTestData()
        {
            var now = DateTime.UtcNow;

            return Enumerable.Range(0, 10000)
                .Select(i => new RawRecord
                {
                    Id = i,
                    ProfileDeviation = 5.5 + i/1000f,
                    Timestamp = now.AddMilliseconds(i).ToOADate(),
                    Thickness = 0.2 + i/1000f

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
