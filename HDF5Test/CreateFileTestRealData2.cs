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

            Stopwatch s = new Stopwatch();
            s.Start();

            using var writer = H5DataSetWriter.CreateOneDimensionalDataSetWriter<RawRecordHelper.SRawRecord>(file, "RawRecords", RawRecordHelper.CreateH5Type);
            using var context = new TvlAltContext();

            long measurementId = 12019;

            // complicated query to flatten the explicit one-many but implied one-one relationships
            // so we end up with inner joins
            context
                .RawRecords
                .Where(r => r.MeasurementId == measurementId)
                .Buffer(20)
                .ForEach(rg =>
                {
                    writer.Write(rg.Select(r => RawRecordHelper.Convert(r)));
                });

            s.Stop();
            Console.WriteLine($"Time elapsed: {s.Elapsed}. Rows written = {writer.CurrentPosition}");
        }
    }
}

