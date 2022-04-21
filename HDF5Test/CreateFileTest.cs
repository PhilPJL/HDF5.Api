using HDF.PInvoke;
using HDF5Api;
using PulseData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HDF5Test
{
    internal static class CreateFileTest
    {
        internal static async Task CreateFileAsync(IEnumerable<int> measurementIds, int minutes)
        {
            await Task.Yield();

            uint compressionLevel = 0;

            // Create file and group
            using var file = H5File.Create(@"test-data.h5");

            foreach (var measurementId in measurementIds)
            {
                using var group = file.CreateGroup($"Measurement:{measurementId}");

                Console.WriteLine($"Writing to file test-data.h5, group Measurement:{measurementId}.  Minutes of data = {minutes}.  Compression level {compressionLevel}.");
                Console.WriteLine();

                using var altContext = new TvlAltContext();
                using var systemContext = new TvlSystemContext();

                var startDateTime = altContext.Profiles
                    .AsNoTracking()
                    .Select(p => new
                    {
                        ProfileId = p.Id,
                        p.RawRecord
                    })
                    .SelectMany(pr => pr.RawRecord.Waveforms.Select(w => new
                    {
                        WaveformId = w.Id,
                        pr.ProfileId,
                        RawRecordId = pr.RawRecord.Id,
                        pr.RawRecord.Timestamp,
                        pr.RawRecord.MeasurementId
                    }))
                    .GroupBy(r => r.MeasurementId)
                    .Select(g => new
                    {
                        MeasurementId = g.Key,
                        RecordCount = g.Count(),
                        MinTimestamp = g.Min(x => x.Timestamp),
                        MaxTimestamp = g.Max(x => x.Timestamp)
                    })
                    .Where(m => m.MeasurementId == measurementId)
                    .Select(m => m.MinTimestamp)
                    .Single();

                var endDateTime = startDateTime + TimeSpan.FromMinutes(minutes); ;

                Console.WriteLine($"Start date={startDateTime}, end={endDateTime}");

                using (new DisposableStopWatch($"Creating group: Measurement:{measurementId}"))
                {
                    var exporter = new HdfExporter(measurementId, startDateTime, endDateTime, 100, compressionLevel);

                    //using (var scope = HdfExporter.GetTransactionScope())
                    {
                        var (numRawRecords, numIntervalRecords, numWaveformRecords, numProfileRecords) = exporter.GetRowCounts();

                        Console.WriteLine($"RawRecords={numRawRecords}, IntervalRecords={numIntervalRecords}, WaveformRecords={numWaveformRecords}, ProfileRecords={numProfileRecords}");

                        //exporter.CreateRawRecordsDataSet2(group);

                        exporter.CreateRawRecordsDataSet(group);
                        exporter.CreateWaveformsDataSet(group);
                        exporter.CreateProfilesDataSet(group);
                        exporter.CreateIntervalsDataSet(group);

                        //var t1 = Task.Run(() => exporter.CreateRawRecordsDataSet(group));
                        //var t2 = Task.Run(() => exporter.CreateWaveformsDataSet(group));
                        //var t3 = Task.Run(() => exporter.CreateProfilesDataSet(group));
                        //var t4 = Task.Run(() => exporter.CreateIntervalsDataSet(group));

                        //await Task.WhenAll(t1, t2, t3, t4);

                        exporter.CreateMeaurementAttributes(group);
                        exporter.CreateMeasurementConfigurationAttributes(group);
                        exporter.CreateInstallationConfigurationAttributes(group);

                        exporter.CreateBladeProfileNamesDataSet(group);
                        exporter.CreateBladeReferencesDataSet(group);
                        exporter.CreateBladeProfileCalibrationsDataSet(group);
                        exporter.CreateBladeProfileCalibrationSetsDataSet(group);
                    }
                }
            }
        }
    }

    class DisposableStopWatch : Disposable
    {
        readonly Stopwatch stopwatch = new();

        public DisposableStopWatch(string name, Func<int> getPosition = null)
        {
            stopwatch.Start();
            Name = name;
            GetPosition = getPosition;
        }

        private string Name { get; }
        private Func<int> GetPosition { get; }

        public void ShowRowsWritten(bool show)
        {
            if (show)
            {
                Console.WriteLine($"{Name} - time elapsed: {stopwatch.Elapsed}. Rows written = {GetPosition()}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            stopwatch.Stop();

            if (GetPosition != null)
            {
                Console.WriteLine($"{Name} - Complete. Time elapsed: {stopwatch.Elapsed}. Rows written = {GetPosition()}");
            }
            else
            {
                Console.WriteLine($"{Name} - Complete. Time elapsed: {stopwatch.Elapsed}.");
            }

            Console.WriteLine();
        }
    }
}

