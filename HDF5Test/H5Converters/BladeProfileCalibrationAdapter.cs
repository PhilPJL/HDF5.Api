using HDF.PInvoke;
using HDF5Api;
using HDF5Api.Disposables;
using PulseData.TvlSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="BladeProfileCalibration"/>.
    /// </summary>
    public sealed class BladeProfileCalibrationAdapter : H5TypeAdapter<BladeProfileCalibration, BladeProfileCalibrationAdapter.SBladeProfileCalibration>
    {
        private const int CorrectionBlobSize = 16384;
        private const int CorrectionBlobTypeSize = sizeof(double);
        private BladeProfileCalibrationAdapter() { }

        protected override unsafe SBladeProfileCalibration Convert(BladeProfileCalibration source)
        {
            var result = new SBladeProfileCalibration
            {
                Id = source.Id,
                BladeProfileCalibrationSetId = source.BladeProfileCalibrationSetId,
                ProfileValue = source.ProfileValue,
                CorrectionValuesLength = (source.CorrectionValues?.Length ?? 0) / CorrectionBlobTypeSize
            };

            unsafe
            {
                CopyBlob(source.CorrectionValues, result.CorrectionValues, CorrectionBlobSize, CorrectionBlobTypeSize);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var correctionValuesType = H5Type.CreateDoubleArrayType(CorrectionBlobSize / CorrectionBlobTypeSize);

            return H5Type
                .CreateCompoundType<SBladeProfileCalibration>()
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.Id), H5T.NATIVE_INT64)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.ProfileValue), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.CorrectionValuesLength), H5T.NATIVE_INT32)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.CorrectionValues), correctionValuesType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeProfileCalibration
        {
            public int Id;
            public int BladeProfileCalibrationSetId;
            public double ProfileValue;
            public int CorrectionValuesLength;
            public fixed byte CorrectionValues[CorrectionBlobSize];
        }

        public static IH5TypeAdapter<BladeProfileCalibration> Default { get; } = new BladeProfileCalibrationAdapter();
    }

    /// <summary>
    /// A type converter for <see cref="BladeProfileCalibration"/>.
    /// </summary>
    public sealed class BladeProfileCalibrationVariableAdapter : H5TypeAdapter<BladeProfileCalibration>
    {
        private readonly int CorrectionBlobSize;
        private readonly int CorrectionBlobTypeSize;

        private BladeProfileCalibrationVariableAdapter(int blobLength, int blobTypeSize)
        {
            CorrectionBlobSize = blobLength;
            CorrectionBlobTypeSize = blobTypeSize;
        }

        private SBladeProfileCalibration Convert(BladeProfileCalibration source)
        {
            var result = new SBladeProfileCalibration
            {
                Id = source.Id,
                BladeProfileCalibrationSetId = source.BladeProfileCalibrationSetId,
                ProfileValue = source.ProfileValue
            };

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var correctionValuesType = H5Type.CreateDoubleArrayType(CorrectionBlobSize / CorrectionBlobTypeSize);

            int blobOffset = Marshal.SizeOf<SBladeProfileCalibration>();

            return H5Type
                .CreateCompoundType<SBladeProfileCalibration>()
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.Id), H5T.NATIVE_INT64)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.ProfileValue), H5T.NATIVE_DOUBLE)
                .Insert("CorrectionValues", blobOffset, correctionValuesType);
        }

        public override void WriteChunk(Action<IntPtr> write, IEnumerable<BladeProfileCalibration> inputRecords)
        {
            var records = inputRecords.Select(Convert).ToList();
            var blobs = inputRecords.Select(ir => ir.CorrectionValues).ToList();

            // TODO: improve
            // if any record.Value.Length != CorrectionBlobSize, throw
            if(blobs.Any(r => r.Length != CorrectionBlobSize))
            {
                throw new InvalidOperationException($"Wrong length");
            }

            int structSize = Marshal.SizeOf<SBladeProfileCalibration>();
            int bufferSize = (structSize + CorrectionBlobSize) * records.Count;

            using var buffer = new GlobalMemory(bufferSize);

            unsafe
            {
                var remainingBufferSize = bufferSize;

                for (int i = 0; i < records.Count; i++)
                {
                    using var pinnedRecord = new PinnedObject(records[i]);
                    using var pinnedBlob = new PinnedObject(blobs[i]);

                    // Copy SBladeProfileCalibration
                    // TODO: point at correct place in buffer
                    Buffer.MemoryCopy(pinnedRecord, CurrentBufferPosition(), remainingBufferSize, structSize);

                    remainingBufferSize -= structSize;

                    // Copy correction values
                    // TODO: point at correct place in buffer
                    Buffer.MemoryCopy(pinnedBlob, CurrentBufferPosition(), remainingBufferSize, CorrectionBlobSize);

                    remainingBufferSize -= CorrectionBlobSize;
                }

                void* CurrentBufferPosition()
                {
                    return IntPtr.Add(buffer.IntPtr, bufferSize - remainingBufferSize).ToPointer();
                }
            }

            write(buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeProfileCalibration
        {
            public int Id;
            public int BladeProfileCalibrationSetId;
            public double ProfileValue;
        }

        public static IH5TypeAdapter<BladeProfileCalibration> Create(int blobLength, int blobTypeSize) => new BladeProfileCalibrationVariableAdapter(blobLength, blobTypeSize);
    }
}
