using HDF.PInvoke;
using HDF5Api;
using HDF5Test.H5TypeHelpers;
using PulseData;
using System;
using System.Diagnostics;
using System.Linq;

namespace HDF5Test
{
    internal static class CreateFileTestRealData2
    {
        // Working demo of creating chunked dataset with compound type.
        internal unsafe static void CreateFile()
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");

            // Create file
            using var file = H5File.Create(@"test-realdata.h5", H5F.ACC_TRUNC);
            Console.WriteLine($"Created file: {file}");

            long measurementId = 12019;
            using var group = file.CreateGroup($"Measurement:{measurementId}");

            using var rawRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter<RawRecordHelper.SRawRecord>(group, "RawRecords", RawRecordHelper.CreateH5Type);
            using var intervalRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter<IntervalRecordHelper.SIntervalRecord>(group, "IntervalRecords", RawRecordHelper.CreateH5Type);
            using var context = new TvlAltContext();

            Stopwatch s = new Stopwatch();
            s.Start();

            // TODO: async queryable/cancellable
            // TODO: overlap?

            context
                .RawRecords
                .Where(r => r.MeasurementId == measurementId)
                .Buffer(20)
                .ForEach(rg =>
                {
                    rawRecordWriter.Write(rg.Select(r => RawRecordHelper.Convert(r)));
                });

            s.Stop();
            Console.WriteLine($"Time elapsed: {s.Elapsed}. RawRecord rows written = {rawRecordWriter.CurrentPosition}");

            s.Start();

            // TODO: simplify query
            context
                .IntervalRecords
                .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == measurementId))
                .Buffer(20)
                .ForEach(rg =>
                {
                    intervalRecordWriter.Write(rg.Select(r => IntervalRecordHelper.Convert(r)));
                });

            s.Stop();
            Console.WriteLine($"Time elapsed: {s.Elapsed}. IntervalRecord rows written = {intervalRecordWriter.CurrentPosition}");
        }
    }
}

