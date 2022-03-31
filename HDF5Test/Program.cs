using HDF.PInvoke;
using System;
using System.Runtime.InteropServices;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            const int Init = -1;
            var fileId = Init;
            var rawRecordTypeId = Init;
            var spaceId = Init;
            var dataSetId = Init;
            var groupId = Init;
            var propId = Init;

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
                H5T.insert(rawRecordTypeId, "MeasurementId", Marshal.OffsetOf<RawRecord>("MeasurementId"), H5T.NATIVE_INT32);
                H5T.insert(rawRecordTypeId, "Thickness", Marshal.OffsetOf<RawRecord>("Thickness"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "ProfileDeviation", Marshal.OffsetOf<RawRecord>("ProfileDeviation"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "ProfileHeight", Marshal.OffsetOf<RawRecord>("ProfileHeight"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "ZPosition", Marshal.OffsetOf<RawRecord>("ZPosition"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "IntervalId", Marshal.OffsetOf<RawRecord>("IntervalId"), H5T.NATIVE_INT64);
                H5T.insert(rawRecordTypeId, "PulseOffset", Marshal.OffsetOf<RawRecord>("PulseOffset"), H5T.NATIVE_DOUBLE);
                H5T.insert(rawRecordTypeId, "ReferenceOffset", Marshal.OffsetOf<RawRecord>("ReferenceOffset"), H5T.NATIVE_INT64);

                spaceId = H5S.create_simple(1, new ulong[] { 1 }, new ulong[] { H5S.UNLIMITED });
                Console.WriteLine($"Created space: {spaceId}");

                // create a group
                groupId = H5G.create(fileId, "Data");
                Console.WriteLine($"Created group: {groupId}");

                propId = H5P.create(H5P.DATASET_CREATE);
                Console.WriteLine($"Created prop: {propId}");

                int err1 = H5P.set_chunk(propId, 1, new ulong[] { 1 });

                if (err1 < 0)
                {
                    Console.WriteLine($"Error H5P.set_chunk={err1}.");
                }

                // create a dataset
                dataSetId = H5D.create(groupId, "RawRecords", rawRecordTypeId, spaceId, H5P.DEFAULT, propId);
                Console.WriteLine($"Created data set: {dataSetId}");

                var records = GetTestData();
                GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

                try
                {
                    H5D.write(dataSetId, rawRecordTypeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinnedBuffer.AddrOfPinnedObject());
                }
                finally
                {
                    pinnedBuffer.Free();
                }
            }
            finally
            {
                // close file
                int result;

                if (fileId > -1)
                {
                    result = H5F.close(fileId);

                    Console.WriteLine($"Closed file: {fileId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5F.close={result}.");
                    }
                }

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

                if (propId > -1)
                {
                    // close space
                    result = H5P.close(propId);

                    Console.WriteLine($"Closed prop: {propId}");

                    if (result < 0)
                    {
                        Console.WriteLine($"Error H5S.close={result}.");
                    }
                }
            }
        }

        static RawRecord[] GetTestData()
        {
            return new RawRecord[]
            {
                new RawRecord
                {
                    Id = 1,
                    ProfileDeviation = 5.6,
                    Thickness = 0.2
                }
            };
        }
        /*        public static long CreateType(Type t)
                {
                    var size = Marshal.SizeOf(t);
                    var float_size = Marshal.SizeOf(typeof(float));
                    var int_size = Marshal.SizeOf(typeof(int));
                    var typeId = H5T.create(H5T.class_t.COMPOUND, new IntPtr(size));
                    if (t == typeof(byte))
                    {
                        H5T.insert(typeId, t.Name, IntPtr.Zero, GetDatatype(t));
                        return typeId;
                    }
                    var compoundInfo = GetCompoundInfo(t);
                    foreach (var cmp in compoundInfo)
                    {
                        //Console.WriteLine(string.Format("{0}  {1}", cmp.name, cmp.datatype));
                        // Lines below don't produce an error message but hdfview can't read compounds properly
                        //var typeLong = GetDatatype(cmp.type);
                        //H5T.insert(typeId, cmp.name, Marshal.OffsetOf(t, cmp.name), typeLong);
                        H5T.insert(typeId, cmp.displayName, Marshal.OffsetOf(t, cmp.name), cmp.datatype);
                    }
                    return typeId;
                }*/
    }

    //CTSWaveformAndProfileDatabaseSpectra
    [StructLayout(LayoutKind.Sequential)]
    struct RawRecord
    {
        public long Id;
        public int MeasurementId;
        //	public DateTime Timestamp { get; set; }
        public double Thickness;
        public double ProfileDeviation;
        public double ProfileHeight;
        public double ZPosition;
        public long IntervalId;
        public double PulseOffset;
        public long ReferenceOffset;
    }
}
