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

            int maxRows = 100;
            long measurementId = 12019;
            using var group = file.CreateGroup($"Measurement:{measurementId}");

            using var altContext = new TvlAltContext();
            using var systemContext = new TvlSystemContext();

            // TODO: async queryable/cancellable
            // TODO: overlap?

            using var rawRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "RawRecords", RawRecordConverter.Default);
            using (new DisposableStopWatch("RawRecord", () => rawRecordWriter.CurrentPosition))
            {
                altContext
                    .RawRecords
                    .Where(r => r.MeasurementId == measurementId)
                    .Take(maxRows)
                    .Buffer(20)
                    .ForEach(rg =>
                    {
                        rawRecordWriter.Write(rg);
                    });
            }

            using var intervalRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "IntervalRecords", IntervalRecordConverter.Default);
            using (new DisposableStopWatch("IntervalRecord", () => intervalRecordWriter.CurrentPosition))
            {
                altContext
                    .IntervalRecords
                    .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == measurementId))
                    .Take(maxRows)
                    .Buffer(20)
                    .ForEach(rg =>
                    {
                        intervalRecordWriter.Write(rg);
                    });
            }

            using var waveformWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Waveforms", WaveformConverter.Default);
            using (new DisposableStopWatch("Waveform", () => waveformWriter.CurrentPosition))
            {
                altContext
                    .Waveforms
                    .Where(w => w.RawRecord.MeasurementId == measurementId)
                    .Take(maxRows)
                    .Buffer(20)
                    .ForEach(rg =>
                    {
                        waveformWriter.Write(rg);
                    });
            }

            using var profileRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Profiles", ProfileConverter.Default);
            using (new DisposableStopWatch("Profile", () => profileRecordWriter.CurrentPosition))
            {
                altContext
                    .Profiles
                    .Where(p => p.RawRecord.MeasurementId == measurementId)
                    .Take(maxRows)
                    .Buffer(20)
                    .ForEach(rg =>
                    {
                        profileRecordWriter.Write(rg);
                    });
            }

            using var measurementConfigurationWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "MeasurementConfigurations", MeasurementConfigurationConverter.Default);
            using (new DisposableStopWatch("MeasurementConfiguration", () => measurementConfigurationWriter.CurrentPosition))
            {
                systemContext
                    .MeasurementConfigurations
                    .Where(mc => mc.Measurements.Any(m => m.Id == measurementId))
                    .Take(maxRows)
                    .Buffer(20)
                    .ForEach(rg =>
                    {
                        measurementConfigurationWriter.Write(rg);
                    });
            }
        }
    }

    class DisposableStopWatch : Disposable
    {
        readonly Stopwatch stopwatch = new();

        public DisposableStopWatch(string name, Func<int> getPosition)
        {
            stopwatch.Start();
            Name = name;
            GetPosition = getPosition;
        }

        private string Name { get; }
        private Func<int> GetPosition { get; }

        protected override void Dispose(bool disposing)
        {
            stopwatch.Stop();
            Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}. {Name} rows written = {GetPosition()}");
        }
    }
}

