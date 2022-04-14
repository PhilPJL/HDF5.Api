using HDF.PInvoke;
using HDF5Api;
using PulseData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HDF5Test
{
    internal static class CreateFileTest
    {
        internal static async Task CreateFileAsync(IEnumerable<int> measurementIds, int minutes)
        {
            await Task.Yield();

            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");
            Console.WriteLine();

            uint compressionLevel = 1;

            // Create file and group
            using var file = H5File.Create(@"test-data.h5", H5F.ACC_TRUNC);

            foreach (var measurementId in measurementIds)
            {
                using var group = file.CreateGroup($"Measurement:{measurementId}");

                Console.WriteLine($"Writing to file test-data.h5, group Measurement:{measurementId}.  Minutes of data = {minutes}.  Compression level {compressionLevel}.");
                Console.WriteLine();

                using var altContext = new TvlAltContext();
                using var systemContext = new TvlSystemContext();

                var endDateTime = DateTime.UtcNow;
                var startDateTime = endDateTime - TimeSpan.FromMinutes(minutes);

                Console.WriteLine($"Start date={startDateTime}, end={endDateTime}");

                using (new DisposableStopWatch($"Creating group: Measurement:{measurementId}"))
                {
                    var exporter = new HdfExporter(measurementId, startDateTime, endDateTime, 100, compressionLevel);

                    exporter.GetRowCounts();

                    exporter.CreateRawRecordsDataSet(group);
                    exporter.CreateIntervalsDataSet(group);
                    exporter.CreateWaveformsDataSet(group);
                    exporter.CreateProfilesDataSet(group);

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

