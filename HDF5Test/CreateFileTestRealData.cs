using HDF.PInvoke;
using HDF5Api;
using PulseData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test
{
    internal static class CreateFileTestRealData
    {
        // Working demo of creating chunked dataset with compound type.
        internal unsafe static void CreateFile()
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");

            // Create file
            using var file = H5File.Create(@"test-realdata.h5", H5F.ACC_TRUNC);
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
            var rand = new Random(Environment.TickCount);

            using (var context = new TvlAltContext())
            {
                long measurementId = 12018;

                context.RawRecords
                    .Select(r => new RawRecordDB
                    {
                        Id = r.Id,
                        MeasurementId = r.MeasurementId,
                        Timestamp = r.Timestamp,
                        Thickness = r.Thickness,
                        ProfileDeviation = r.ProfileDeviation,
                        ProfileHeight = r.ProfileHeight,
                        ZPosition = r.ZPosition,
                        IntervalId = r.IntervalId,
                        PulseOffset = r.PulseOffset,
                        ReferenceOffset = r.ReferenceOffset,
                        Waveforms = r.Waveforms.Select(w => w.Values),
                        Profiles = r.Profiles.Select(p => p.Values)
                    })
                    .Where(r => r.MeasurementId == measurementId)
                    .OrderByDescending(r => r.Id)
                    .Take(5000)
                    .Buffer(100)
                    .ForEach(rg =>
                    {
                        var records = GetRecords(rg.ToArray());

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
                    });

            }

            s.Stop();
            Console.WriteLine($"Time elapsed: {s.Elapsed}. Total rows {extent[0]}.");
        }

        unsafe static RawRecord[] GetRecords(RawRecordDB[] input)
        {
            var result = new RawRecord[input.Length];

            for (int i = 0; i < input.Count(); i++)
            {
                result[i].Id = input[i].Id;
                result[i].MeasurementId = input[i].MeasurementId;
                result[i].Timestamp = input[i].Timestamp.ToOADate();
                result[i].Thickness = input[i].Thickness ?? 0;
                result[i].ProfileDeviation = input[i].ProfileDeviation ?? 0;
                result[i].ProfileHeight = input[i].ProfileHeight ?? 0;
                result[i].ZPosition = input[i].ZPosition ?? 0;
                result[i].IntervalId = input[i].IntervalId ?? 0;
                result[i].PulseOffset = input[i].PulseOffset ?? 0;
                result[i].ReferenceOffset = input[i].ReferenceOffset ?? 0;

                // TODO: block copy etc
                fixed (byte* p1 = result[i].Waveform)
                {
                    var wf = input[i].Waveforms.Single();

                    //if (wf.Length != RawRecord.waveformBlobSize)
                    //{
                    //    Console.WriteLine($"Profile: MeasId {input[i].MeasurementId} Expected length {RawRecord.waveformBlobSize}, got {wf.Length}");
                    //}


                    Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(wf, 0).ToPointer(), p1, RawRecord.waveformBlobSize, wf.Length);

                    //for (int pos = 0; pos < wf.Length; pos++)
                    //{
                    //    p1[pos] = wf[pos];
                    //}
                }

                fixed (byte* p1 = result[i].Profile1)
                fixed (byte* p2 = result[i].Profile2)
                {
                    var pf = input[i].Profiles.Single();

                    //if (pf.Length != RawRecord.profileBlobSize)
                    //{
                    //    Console.WriteLine($"Profile: MeasId {input[i].MeasurementId} Expected length {RawRecord.profileBlobSize}, got {pf.Length}");
                    //}

                    Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), p1, RawRecord.profileBlobSize / 2, pf.Length / 2);
                    Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, pf.Length / 2).ToPointer(), p2, RawRecord.profileBlobSize / 2, pf.Length / 2);

                    //for (int pos = 0; pos < pf.Length / 2; pos++)
                    //{
                    //    p1[pos] = pf[pos];
                    //    p2[pos] = pf[pos + pf.Length / 2];
                    //}
                }
            }

            return result;
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

        internal class RawRecordDB
        {
            public long Id { get; set; }
            public int MeasurementId { get; set; }
            public DateTime Timestamp { get; set; }
            public double? Thickness { get; set; }
            public double? ProfileDeviation { get; set; }
            public double? ProfileHeight { get; set; }
            public double? ZPosition { get; set; }
            public long? IntervalId { get; set; }
            public double? PulseOffset { get; set; }
            public double? ReferenceOffset { get; set; }
            public IEnumerable<byte[]> Waveforms { get; set; }
            public IEnumerable<byte[]> Profiles { get; set; }
        }
    }
}

