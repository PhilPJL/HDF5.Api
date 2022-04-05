﻿using HDF.PInvoke;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Handle = System.Int64;

namespace HDF5Test
{
    static class Program
    {
        static void Main()
        {
            try
            {
                CreateFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        unsafe static void CreateFile()
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");

            // Create file
            using var fileId = H5File.Create(@"test2.h5", H5F.ACC_TRUNC);
            Console.WriteLine($"Created file: {fileId}");

            // setup compound type
            // TODO: how to have variable length arrays per run
            int size = Marshal.SizeOf<RawRecord>();
            Console.WriteLine($"Datatype size = {size}, {size - 32768 - 16384}");

            using var rawRecordTypeId = H5Type.CreateCompoundType(size);
            Console.WriteLine($"Created type: {rawRecordTypeId}");

            H5Type.Insert(rawRecordTypeId, "Id", Marshal.OffsetOf<RawRecord>("Id"), H5T.NATIVE_INT64);
            H5Type.Insert(rawRecordTypeId, "Measurement Id", Marshal.OffsetOf<RawRecord>("MeasurementId"), H5T.NATIVE_INT32);
            H5Type.Insert(rawRecordTypeId, "Timestamp", Marshal.OffsetOf<RawRecord>("Timestamp"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Thickness", Marshal.OffsetOf<RawRecord>("Thickness"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Profile deviation", Marshal.OffsetOf<RawRecord>("ProfileDeviation"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Profile height", Marshal.OffsetOf<RawRecord>("ProfileHeight"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Z position", Marshal.OffsetOf<RawRecord>("ZPosition"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Interval Id", Marshal.OffsetOf<RawRecord>("IntervalId"), H5T.NATIVE_INT64);
            H5Type.Insert(rawRecordTypeId, "Pulse offset", Marshal.OffsetOf<RawRecord>("PulseOffset"), H5T.NATIVE_DOUBLE);
            H5Type.Insert(rawRecordTypeId, "Reference offset", Marshal.OffsetOf<RawRecord>("ReferenceOffset"), H5T.NATIVE_INT64);

            using var byteArrayId1 = H5Type.CreateByteArrayType(32768);
            H5Type.Insert(rawRecordTypeId, "Array1", Marshal.OffsetOf<RawRecord>("Array1"), byteArrayId1);
            using var byteArrayId2 = H5Type.CreateByteArrayType(16384);
            H5Type.Insert(rawRecordTypeId, "Array2", Marshal.OffsetOf<RawRecord>("Array2"), byteArrayId2);

            int chunkSize = 100;

            // create a dataspace - single dimension 1 x unlimited
            // this is the chunk size that the dataset extends itself by
            var dims = new ulong[] { (ulong)chunkSize };
            var maxdims = new ulong[] { H5S.UNLIMITED };

            // a dataspace defining the chunk size of our data set
            // Q: why do we need a memory space with chunk size, and a property list with the same chunk size - or do we?
            using var memorySpaceId = H5Space.CreateSimple(1, dims, maxdims);
            Console.WriteLine($"Created space: {memorySpaceId}");

            // create a dataset-create property list
            using var propListId = H5PropertyList.Create(H5P.DATASET_CREATE);
            Console.WriteLine($"Created prop: {propListId}");
            // 1) allow chunking - doesn't work without this. From user guide: HDF5 requires the use of chunking when defining extendable datasets
            H5PropertyList.SetChunk(propListId, 1, dims);
            // 2) enable compression
            H5PropertyList.EnableDeflateCompression(propListId, 6);

            // create a group name 'Data'
            using var groupId = H5Group.Create(fileId, "Data");
            Console.WriteLine($"Created group: {groupId}");

            // create a dataset named 'RawRecords' in group 'Data' with our record type and chunk size
            using var dataSet = H5DataSet.Create(groupId, "RawRecords", rawRecordTypeId, memorySpaceId, propListId);
            Console.WriteLine($"Created data set: {dataSet}");

            Stopwatch s = new Stopwatch();
            s.Start();

            var extent = new ulong[] { 0 };
            var rand = new Random(Environment.TickCount);

            for (int i = 0; i < 50; i++)
            {
                var records = GetTestData(i, rand.Next(10, 200));

                Console.WriteLine(records.Length);

                GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

                try
                {
                    // record current position for the hyperslab window
                    int currentPosition = (int)extent[0];

                    // extend the dataset to accept this chunk
                    extent[0] = (ulong)(currentPosition + records.Length);
                    H5DataSet.SetExtent(dataSet, extent);

                    // move the hyperslab window
                    using var fileSpace = H5DataSet.GetFileSpace(dataSet);
                    fileSpace.SelectHyperslab(currentPosition, records.Length);

                    // match the space to length of records retrieved
                    // if using standard length chunks (say 100) then only need to change this for the final write
                    using var recordSpace = H5Space.CreateSimple(1, new ulong[] { (ulong)records.Length }, maxdims);
                    dataSet.Write(rawRecordTypeId, recordSpace, fileSpace, pinnedBuffer.AddrOfPinnedObject());
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
    }

    //CTSWaveformAndProfileDatabaseSpectra
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct RawRecord
    {
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

            for (int i = 0; i < 32768; i++)
            {
                Array1[i] = (byte)i;
            }

            for (int i = 0; i < 16384; i++)
            {
                Array2[i] = (byte)(byte.MaxValue - (byte)i);
            }
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
        public long ReferenceOffset;
        public fixed byte Array1[32768];
        public fixed byte Array2[16384];
    }
}
