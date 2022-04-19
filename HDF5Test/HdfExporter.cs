using HDF5Api;
using HDF5Test.H5TypeHelpers;
using PulseData;
using System;
using System.Linq;
using System.Transactions;

namespace HDF5Test
{
    internal class HdfExporter
    {
        public int MeasurementId { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public int ChunkSize { get; }
        public uint CompressionLevel { get; }

        private bool logTimePerChunk = true;

        public static TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);
        }

        public HdfExporter(int measurementId, DateTime startDateTime, DateTime endDateTime, int chunkSize = 100, uint compressionLevel = 1)
        {
            MeasurementId = measurementId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            ChunkSize = chunkSize;
            CompressionLevel = compressionLevel;
        }

        public (int numRawRecords, int numIntervalRecords, int numWaveformRecords, int numProfileRecords) GetRowCounts()
        {
            using var altContext = new TvlAltContext();
            //using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            var numRawRecords = altContext
                .RawRecords
                .AsNoTracking()
                .Where(r => r.MeasurementId == MeasurementId)
                .Where(r => r.Timestamp >= StartDateTime)
                .Where(r => r.Timestamp <= EndDateTime)
                .Count();

            var numIntervalRecords = altContext
                .IntervalRecords
                .AsNoTracking()
                .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == MeasurementId))
                .Where(r => r.Timestamp >= StartDateTime)
                .Where(r => r.Timestamp <= EndDateTime)
                .Count();

            var numWaveformRecords = altContext
                .Waveforms
                .AsNoTracking()
                .Where(w => w.RawRecord.MeasurementId == MeasurementId)
                .Where(w => w.RawRecord.Timestamp >= StartDateTime)
                .Where(w => w.RawRecord.Timestamp <= EndDateTime)
                .Count();

            var numProfileRecords = altContext
                .Profiles
                .AsNoTracking()
                .Where(p => p.RawRecord.MeasurementId == MeasurementId)
                .Where(p => p.RawRecord.Timestamp >= StartDateTime)
                .Where(p => p.RawRecord.Timestamp <= EndDateTime)
                .Count();

            scope.Complete();

            return (numRawRecords, numIntervalRecords, numWaveformRecords, numProfileRecords);
        }

        public void CreateRawRecordsDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Raw records
            using var altContext = new TvlAltContext();
            using var scope = GetTransactionScope();

            using var rawRecordWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(location, "RawRecords", RawRecordAdapter.Default, CompressionLevel, ChunkSize);

            using (var sw = new DisposableStopWatch("RawRecord", () => rawRecordWriter.RowsWritten))
            {
                altContext
                    .RawRecords
                    .AsNoTracking()
                    .Where(r => r.MeasurementId == MeasurementId)
                    .Where(r => r.Timestamp >= StartDateTime)
                    .Where(r => r.Timestamp <= EndDateTime)
                    .Buffer(ChunkSize)
                    .ForEach(rg =>
                    {
                        rawRecordWriter.Write(rg);
                        sw.ShowRowsWritten(logTimePerChunk);
                    });
            }

            scope.Complete();
        }

        public void CreateIntervalsDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Interval records
            using var altContext = new TvlAltContext();
            using var scope = GetTransactionScope();

            using var intervalRecordWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(location, "IntervalRecords", IntervalRecordAdapter.Default, CompressionLevel, ChunkSize);

            using (var sw = new DisposableStopWatch("IntervalRecord", () => intervalRecordWriter.RowsWritten))
            {
                altContext
                    .IntervalRecords
                    .AsNoTracking()
                    .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == MeasurementId))
                    .Where(r => r.Timestamp >= StartDateTime)
                    .Where(r => r.Timestamp <= EndDateTime)
                    .Buffer(ChunkSize)
                    .ForEach(rg =>
                    {
                        intervalRecordWriter.Write(rg);
                        sw.ShowRowsWritten(logTimePerChunk);
                    });
            }

            scope.Complete();
        }

        public void CreateWaveformsDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Waveforms
            using var altContext = new TvlAltContext();
            using var scope = GetTransactionScope();

            var waveformValuesBlobLength = altContext.Waveforms
                .AsNoTracking()
                .Where(p => p.RawRecord.MeasurementId == MeasurementId)
                .Select(p => p.Values.OctetLength())
                .FirstOrDefault();

            if (waveformValuesBlobLength > 0)
            {
                Console.WriteLine($"Waveform: blob length {waveformValuesBlobLength}");

                using var waveformWriter = H5DataSetWriter
                    .CreateOneDimensionalDataSetWriter(location, "Waveforms", WaveformAdapter.Create(waveformValuesBlobLength, sizeof(double)), CompressionLevel, ChunkSize);

                using (var sw = new DisposableStopWatch("Waveform", () => waveformWriter.RowsWritten))
                {
                    altContext
                        .Waveforms
                        .AsNoTracking()
                        .Where(w => w.RawRecord.MeasurementId == MeasurementId)
                        .Where(w => w.RawRecord.Timestamp >= StartDateTime)
                        .Where(w => w.RawRecord.Timestamp <= EndDateTime)
                        .Buffer(ChunkSize)
                        .ForEach(rg =>
                        {
                            waveformWriter.Write(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }
            }
            else
            {
                Console.WriteLine($"WARNING: Unable to determine the waveform length for Waveform.");
            }

            scope.Complete();
        }

        public void CreateProfilesDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Profiles
            using var altContext = new TvlAltContext();
            using var scope = GetTransactionScope();

            // Get length of profile blob for this measurement
            var profileValuesBlobLength = altContext
                .Profiles
                .AsNoTracking()
                .Where(p => p.RawRecord.MeasurementId == MeasurementId)
                .Select(p => p.Values.OctetLength())
                .FirstOrDefault();

            if (profileValuesBlobLength > 0)
            {
                Console.WriteLine($"Profile: blob length {profileValuesBlobLength}");

                using var profileRecordWriter = H5DataSetWriter
                    .CreateOneDimensionalDataSetWriter(location, "Profiles", ProfileAdapter.Create(profileValuesBlobLength, sizeof(double)), CompressionLevel, ChunkSize);

                using (var sw = new DisposableStopWatch("Profile", () => profileRecordWriter.RowsWritten))
                {
                    altContext
                        .Profiles
                        .AsNoTracking()
                        .Where(p => p.RawRecord.MeasurementId == MeasurementId)
                        .Where(p => p.RawRecord.Timestamp >= StartDateTime)
                        .Where(p => p.RawRecord.Timestamp <= EndDateTime)
                        .Buffer(ChunkSize)
                        .ForEach(rg =>
                        {
                            profileRecordWriter.Write(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }
            }
            else
            {
                Console.WriteLine($"WARNING: Unable to determine the profile values length for Profile.");
            }

            scope.Complete();
        }

        public void CreateBladeProfileNamesDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Blade profile
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            using var bladeProfileNameWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(location, "BladeProfileNames", BladeProfileNameAdapter.Default, CompressionLevel, ChunkSize);

            using (var sw = new DisposableStopWatch("BladeProfileName", () => bladeProfileNameWriter.RowsWritten))
            {
                systemContext
                    .BladeProfileNames
                    .AsNoTracking()
                    .Buffer(ChunkSize)
                    .ForEach(rg =>
                    {
                        bladeProfileNameWriter.Write(rg);
                        sw.ShowRowsWritten(logTimePerChunk);
                    });
            }

            scope.Complete();
        }

        public void CreateBladeReferencesDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Blade reference
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            // Get sizes of mirror and blade waveform blobs
            var waveformLengths = systemContext
                .BladeReferences
                .AsNoTracking()
                .Select(b => new { MirrorWaveformLength = b.MirrorWaveform.OctetLength(), BladeWaveformLength = b.BladeWaveform.OctetLength() })
                .FirstOrDefault();

            if (waveformLengths != null)
            {
                int mirrorWaveformBlobLength = waveformLengths.MirrorWaveformLength;
                int bladeWaveformBlobLength = waveformLengths.BladeWaveformLength;

                if (mirrorWaveformBlobLength > 0 && bladeWaveformBlobLength > 0)
                {
                    Console.WriteLine($"Blade reference: blob lengths Blade={bladeWaveformBlobLength}, Mirror={mirrorWaveformBlobLength}");

                    var adapter = BladeReferenceAdapter.Create(bladeWaveformBlobLength, sizeof(double), mirrorWaveformBlobLength, sizeof(double));

                    using var bladeReferenceWriter = H5DataSetWriter
                        .CreateOneDimensionalDataSetWriter(location, "BladeReferences", adapter, CompressionLevel, ChunkSize);

                    using (var sw = new DisposableStopWatch("BladeReference", () => bladeReferenceWriter.RowsWritten))
                    {
                        systemContext
                            .BladeReferences
                            .AsNoTracking()
                            .Buffer(ChunkSize)
                            .ForEach(rg =>
                            {
                                bladeReferenceWriter.Write(rg);
                                sw.ShowRowsWritten(logTimePerChunk);
                            });
                    }
                }
                else
                {
                    Console.WriteLine($"WARNING: Unable to determine the waveform lengths for BladeReference.");
                }
            }
            else
            {
                Console.WriteLine($"WARNING: No BladeReference records found.");
            }

            scope.Complete();
        }

        public void CreateBladeProfileCalibrationsDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Blade profile calibration
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            // Get size of correction values blob
            var correctionValuesBlobLength = systemContext
                        .BladeProfileCalibrations
                        .AsNoTracking()
                        .Select(b => b.CorrectionValues.OctetLength())
                        .FirstOrDefault();

            if (correctionValuesBlobLength > 0)
            {
                Console.WriteLine($"Blade profile calibrations: blob length {correctionValuesBlobLength}");

                var adapter = BladeProfileCalibrationAdapter.Create(correctionValuesBlobLength, sizeof(double));

                using var bladeProfileCalibrationWriter = H5DataSetWriter
                    .CreateOneDimensionalDataSetWriter(location, "BladeProfileCalibrations", adapter, CompressionLevel, ChunkSize);

                using (var sw = new DisposableStopWatch("BladeProfileCalibration", () => bladeProfileCalibrationWriter.RowsWritten))
                {
                    systemContext
                        .BladeProfileCalibrations
                        .AsNoTracking()
                        .Buffer(ChunkSize)
                        .ForEach(rg =>
                        {
                            bladeProfileCalibrationWriter.Write(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }
            }
            else
            {
                Console.WriteLine($"WARNING: Unable to determine the correction values length for BladeProfileCalibration.");
            }

            scope.Complete();
        }

        public void CreateBladeProfileCalibrationSetsDataSet(IH5Location location)
        {
            //////////////////////////////////////
            // Blade profile calibration set
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            using var bladeProfileCalibrationSetWriter = H5DataSetWriter
                .CreateOneDimensionalDataSetWriter(location, "BladeProfileCalibrationSets", BladeProfileCalibrationSetAdapter.Default, CompressionLevel, ChunkSize);

            using (var sw = new DisposableStopWatch("BladeProfileCalibrationSet", () => bladeProfileCalibrationSetWriter.RowsWritten))
            {
                systemContext
                    .BladeProfileCalibrationSets
                    .AsNoTracking()
                    .Buffer(ChunkSize)
                    .ForEach(rg =>
                    {
                        bladeProfileCalibrationSetWriter.Write(rg);
                        sw.ShowRowsWritten(logTimePerChunk);
                    });
            }

            scope.Complete();
        }

        public void CreateMeaurementAttributes(IH5Location location)
        {
            //////////////////////////////////////
            // Measurements
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            var measurements = systemContext
                .Measurements
                .AsNoTracking()
                .Where(m => m.Id == MeasurementId)
                .ToList();

            if (!measurements.Any())
            {
                // Warning: none found
            }
            else
            {
                if (measurements.Count == 1)
                {
                    MeasurementSimpleAttributeWriter.WriteAttributes(location, measurements.Single());
                }
                else
                {
                    // Warning: too many found
                }
            }
        }

        public void CreateInstallationConfigurationAttributes(IH5Location location)
        {
            //////////////////////////////////////
            // Installation configuration - using attributes
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            var configs = systemContext
                        .InstallationConfigurations
                        .AsNoTracking()
                        .Where(ic => ic.Measurements.Any(m => m.Id == MeasurementId))
                        .ToList();

            if (!configs.Any())
            {
                // Warning: none found
            }
            else
            {
                if (configs.Count == 1)
                {
                    InstallationConfigurationSimpleAttributeWriter.WriteAttributes(location, configs.Single());
                }
                else
                {
                    // Warning: too many found
                }
            }

            scope.Complete();
        }

        public void CreateMeasurementConfigurationAttributes(IH5Location location)
        {
            //////////////////////////////////////
            // Measurement configuration - using attributes
            using var systemContext = new TvlSystemContext();
            using var scope = GetTransactionScope();

            var configs = systemContext
                        .MeasurementConfigurations
                        .AsNoTracking()
                        .Where(mc => mc.Measurements.Any(m => m.Id == MeasurementId))
                        .ToList();

            if (!configs.Any())
            {
                // Warning: none found
            }
            else
            {
                if (configs.Count == 1)
                {
                    MeasurementConfigurationSimpleAttributeWriter.WriteAttributes(location, configs.Single());
                }
                else
                {
                    // Warning: too many found
                }
            }

            scope.Complete();
        }
    }
}

