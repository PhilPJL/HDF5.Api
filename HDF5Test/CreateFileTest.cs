using HDF.PInvoke;
using HDF5Api;
using HDF5Test.H5TypeHelpers;
using PulseData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace HDF5Test
{
    internal static class CreateFileTest
    {
        internal static void CreateFile(IEnumerable<int> measurementIds, int maxRows)
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");
            Console.WriteLine();

            int chunkSize = 50;
            bool logTimePerChunk = true;
            uint compressionLevel = 1;

            // Create file and group
            using var file = H5File.Create(@"test-data.h5", H5F.ACC_TRUNC);

            foreach (var measurementId in measurementIds)
            {
                using var group = file.CreateGroup($"Measurement:{measurementId}");

                Console.WriteLine($"Writing to file test-data.h5, group Measurement:{measurementId}.  Max rows {maxRows}.  Compression level {compressionLevel}.");
                Console.WriteLine();

                using var altContext = new TvlAltContext();
                using var systemContext = new TvlSystemContext();

                using (new DisposableStopWatch("Create group."))
                {
                    //////////////////////////////////////
                    // Raw records
                    using var rawRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "RawRecords", RawRecordAdapter.Default, compressionLevel);
                    using (var sw = new DisposableStopWatch("RawRecord", () => rawRecordWriter.RowsWritten))
                    {
                        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                        altContext
                            .RawRecords
                            .AsNoTracking()
                            .Where(r => r.MeasurementId == measurementId)
                            .Take(maxRows)
                            .Buffer(chunkSize)
                            .ForEach(rg =>
                            {
                                rawRecordWriter.WriteChunk(rg);
                                sw.ShowRowsWritten(logTimePerChunk);
                            });
                        scope.Complete();
                    }

                    //////////////////////////////////////
                    // Interval records
                    using var intervalRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "IntervalRecords", IntervalRecordAdapter.Default, compressionLevel);
                    using (var sw = new DisposableStopWatch("IntervalRecord", () => intervalRecordWriter.RowsWritten))
                    {
                        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                        altContext
                            .IntervalRecords
                            .AsNoTracking()
                            .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == measurementId))
                            .Take(maxRows)
                            .Buffer(chunkSize)
                            .ForEach(rg =>
                            {
                                intervalRecordWriter.WriteChunk(rg);
                                sw.ShowRowsWritten(logTimePerChunk);
                            });
                        scope.Complete();
                    }

                    //////////////////////////////////////
                    // Waveforms

                    // Get length of waveform blob for this measurement
                    var firstWaveform = altContext
                        .Waveforms
                        .AsNoTracking()
                        .Where(p => p.RawRecord.MeasurementId == measurementId)
                        .FirstOrDefault();

                    if (firstWaveform != null)
                    {
                        var waveformValuesBlobLength = firstWaveform?.Values?.Length ?? 0;

                        if (waveformValuesBlobLength > 0)
                        {
                            Console.WriteLine($"Waveform: blob length {waveformValuesBlobLength}");

                            using var waveformWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Waveforms", WaveformAdapter.Create(waveformValuesBlobLength, sizeof(double)), compressionLevel);
                            using (var sw = new DisposableStopWatch("Waveform", () => waveformWriter.RowsWritten))
                            {
                                using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                                altContext
                                    .Waveforms
                                    .AsNoTracking()
                                    .Where(w => w.RawRecord.MeasurementId == measurementId)
                                    .Take(maxRows)
                                    .Buffer(chunkSize)
                                    .ForEach(rg =>
                                    {
                                        waveformWriter.WriteChunk(rg);
                                        sw.ShowRowsWritten(logTimePerChunk);
                                    });
                                scope.Complete();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: No waveform values found for waveform id={firstWaveform.Id}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: No Waveform records found for measurement id={measurementId}");
                    }

                    //////////////////////////////////////
                    // Profiles

                    // Get length of profile blob for this measurement
                    var firstProfile = altContext
                        .Profiles
                        .AsNoTracking()
                        .Where(p => p.RawRecord.MeasurementId == measurementId)
                        .FirstOrDefault();

                    if (firstProfile != null)
                    {
                        var profileValuesBlobLength = firstProfile?.Values?.Length ?? 0;

                        if (profileValuesBlobLength > 0)
                        {
                            Console.WriteLine($"Profile: blob length {profileValuesBlobLength}");

                            using var profileRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Profiles", ProfileAdapter.Create(profileValuesBlobLength, sizeof(double)), compressionLevel);
                            using (var sw = new DisposableStopWatch("Profile", () => profileRecordWriter.RowsWritten))
                            {
                                using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                                altContext
                                    .Profiles
                                    .AsNoTracking()
                                    .Where(p => p.RawRecord.MeasurementId == measurementId)
                                    .Take(maxRows)
                                    .Buffer(chunkSize)
                                    .ForEach(rg =>
                                    {
                                        profileRecordWriter.WriteChunk(rg);
                                        sw.ShowRowsWritten(logTimePerChunk);
                                    });
                                scope.Complete();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: No profile values found for profile id={firstProfile.Id}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: No Profile records found for measurement id={measurementId}");
                    }

                    //////////////////////////////////////
                    // Measurements
                    CreateMeaurementAttributes(measurementId, group, systemContext);

                    //////////////////////////////////////
                    // Measurement configuration - using attributes
                    CreateMeasurementConfigurationAttributes(measurementId, group, systemContext);

                    //////////////////////////////////////
                    // Installation configuration - using attributes
                    CreateInstallationConfigurationAttributes(measurementId, group, systemContext);

                    //////////////////////////////////////
                    // Blade profile

                    using var bladeProfileNameWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "BladeProfileNames", BladeProfileNameAdapter.Default, compressionLevel);
                    using (var sw = new DisposableStopWatch("BladeProfileName", () => bladeProfileNameWriter.RowsWritten))
                    {
                        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                        systemContext
                            .BladeProfileNames
                            .AsNoTracking()
                            .Take(maxRows)
                            .Buffer(chunkSize)
                            .ForEach(rg =>
                            {
                                bladeProfileNameWriter.WriteChunk(rg);
                                sw.ShowRowsWritten(logTimePerChunk);
                            });
                        scope.Complete();
                    }

                    //////////////////////////////////////
                    // Blade reference

                    // Get sizes of mirror and blade waveform blobs
                    var firstBladeReference = systemContext.BladeReferences.AsNoTracking().FirstOrDefault();

                    if (firstBladeReference != null)
                    {
                        int mirrorWaveformBlobLength = firstBladeReference?.MirrorWaveform.Length ?? 0;
                        int bladeWaveformBlobLength = firstBladeReference?.BladeWaveform.Length ?? 0;

                        if (mirrorWaveformBlobLength > 0 && bladeWaveformBlobLength > 0)
                        {
                            Console.WriteLine($"Blade reference: blob lengths Blade={bladeWaveformBlobLength}, Mirror={mirrorWaveformBlobLength}");

                            using var bladeReferenceWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "BladeReferences",
                                BladeReferenceAdapter.Create(bladeWaveformBlobLength, sizeof(double), mirrorWaveformBlobLength, sizeof(double)), compressionLevel);
                            using (var sw = new DisposableStopWatch("BladeReference", () => bladeReferenceWriter.RowsWritten))
                            {
                                using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                                systemContext
                                    .BladeReferences
                                    .AsNoTracking()
                                    .Take(maxRows)
                                    .Buffer(chunkSize)
                                    .ForEach(rg =>
                                    {
                                        bladeReferenceWriter.WriteChunk(rg);
                                        sw.ShowRowsWritten(logTimePerChunk);
                                    });
                                scope.Complete();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: BladeReference id={firstBladeReference.Id} has missing mirror and/or blade waveform values.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: No BladeReference records found.");
                    }

                    //////////////////////////////////////
                    // Blade profile calibration

                    // Get size of correction values blob
                    var firstBladeProfileCalibration = systemContext.BladeProfileCalibrations.AsNoTracking().FirstOrDefault();
                    if (firstBladeProfileCalibration != null)
                    {

                        var correctionValuesBlobLength = firstBladeProfileCalibration?.CorrectionValues.Length ?? 0;
                        if (correctionValuesBlobLength > 0)
                        {
                            Console.WriteLine($"Blade profile calibrations: blob length {correctionValuesBlobLength}");

                            using var bladeProfileCalibrationWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group,
                                "BladeProfileCalibrations", BladeProfileCalibrationAdapter.Create(correctionValuesBlobLength, sizeof(double)), compressionLevel);
                            using (var sw = new DisposableStopWatch("BladeProfileCalibration", () => bladeProfileCalibrationWriter.RowsWritten))
                            {
                                using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                                systemContext
                                    .BladeProfileCalibrations
                                    .AsNoTracking()
                                    .Take(maxRows)
                                    .Buffer(chunkSize)
                                    .ForEach(rg =>
                                    {
                                        bladeProfileCalibrationWriter.WriteChunk(rg);
                                        sw.ShowRowsWritten(logTimePerChunk);
                                    });
                                scope.Complete();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: BladeProfileCalibration id={firstBladeProfileCalibration.Id} has missing values.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: No BladeProfileCalibrations records found.");
                    }
                    //////////////////////////////////////
                    // Blade profile calibration set

                    using var bladeProfileCalibrationSetWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "BladeProfileCalibrationSets", BladeProfileCalibrationSetAdapter.Default, compressionLevel);
                    using (var sw = new DisposableStopWatch("BladeProfileCalibrationSet", () => bladeProfileCalibrationSetWriter.RowsWritten))
                    {
                        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                        systemContext
                            .BladeProfileCalibrationSets
                            .AsNoTracking()
                            .Take(maxRows)
                            .Buffer(chunkSize)
                            .ForEach(rg =>
                            {
                                bladeProfileCalibrationSetWriter.WriteChunk(rg);
                                sw.ShowRowsWritten(logTimePerChunk);
                            });
                        scope.Complete();
                    }
                }
            }
        }

        private static void CreateMeaurementAttributes(long measurementId, IH5Location location, TvlSystemContext systemContext)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });

            var measurement = systemContext
                .Measurements
                .AsNoTracking()
                .Where(m => m.Id == measurementId)
                .FirstOrDefault();

            if (measurement != null)
            {
                MeasurementSimpleAttributeWriter.WriteAttributes(location, measurement);
            }

            /*            using var measurementWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Measurements", MeasurementAdapter.Default, compressionLevel);
                        using (var sw = new DisposableStopWatch("Measurement", () => measurementWriter.RowsWritten))
                        {
                            using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                            systemContext
                                .Measurements
                                .AsNoTracking()
                                .Where(m => m.Id == measurementId)
                                .Take(maxRows)
                                .Buffer(chunkSize)
                                .ForEach(rg =>
                                {
                                    measurementWriter.WriteChunk(rg);
                                    sw.ShowRowsWritten(logTimePerChunk);
                                });
                            scope.Complete();
                        }*/
        }

        private static void CreateInstallationConfigurationAttributes(long measurementId, IH5Location location, TvlSystemContext systemContext)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });

            var config = systemContext
                        .InstallationConfigurations
                        .AsNoTracking()
                        .Where(ic => ic.Measurements.Any(m => m.Id == measurementId))
                        .FirstOrDefault();

            //            using var attribute = H5AttributeWriter.CreateAndWriteAttribute(group, "InstallationConfiguration", config, InstallationConfigurationAdapter.Default);

            if (config != null)
            {
                InstallationConfigurationSimpleAttributeWriter.WriteAttributes(location, config);
            }

            scope.Complete();

            /*                using var measurementConfigurationWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "MeasurementConfigurations", MeasurementConfigurationAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("MeasurementConfiguration", () => measurementConfigurationWriter.RowsWritten))
                {
                    using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                    systemContext
                        .MeasurementConfigurations
                        .AsNoTracking()
                        .Where(mc => mc.Measurements.Any(m => m.Id == measurementId))
                        .Take(maxRows)
                        .Buffer(chunkSize)
                        .ForEach(rg =>
                        {
                            measurementConfigurationWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                    scope.Complete();
                }*/
        }

        private static void CreateMeasurementConfigurationAttributes(long measurementId, IH5Location location, TvlSystemContext systemContext)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });

            var config = systemContext
                        .MeasurementConfigurations
                        .AsNoTracking()
                        .Where(mc => mc.Measurements.Any(m => m.Id == measurementId))
                        .FirstOrDefault();

            //            using var attribute = H5AttributeWriter.CreateAndWriteAttribute(group, "MeasurementConfiguration", config, MeasurementConfigurationAdapter.Default);

            if (config != null)
            {
                MeasurementConfigurationSimpleAttributeWriter.WriteAttributes(location, config);
            }

            scope.Complete();


            /*                using var installationConfigurationWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "InstallationConfigurations", InstallationConfigurationAdapter.Default, compressionLevel);
                            using (var sw = new DisposableStopWatch("InstallationConfiguration", () => installationConfigurationWriter.RowsWritten))
                            {
                                using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
                                systemContext
                                    .InstallationConfigurations
                                    .AsNoTracking()
                                    .Where(ic => ic.Measurements.Any(m => m.Id == measurementId))
                                    .Take(maxRows)
                                    .Buffer(chunkSize)
                                    .ForEach(rg =>
                                    {
                                        installationConfigurationWriter.WriteChunk(rg);
                                        sw.ShowRowsWritten(logTimePerChunk);
                                    });
                                scope.Complete();
                            }*/

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

