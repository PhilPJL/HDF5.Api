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
    public sealed class BladeProfileCalibrationAdapter : H5TypeAdapter<BladeProfileCalibration>
    {
        private readonly int CorrectionBlobSize;
        private readonly int CorrectionBlobTypeSize;

        private BladeProfileCalibrationAdapter(int blobLength, int blobTypeSize)
        {
            AssertBlobLengthDivisibleByTypeLength(blobLength, blobTypeSize);

            CorrectionBlobSize = blobLength;
            CorrectionBlobTypeSize = blobTypeSize;
        }

        public override H5Type GetH5Type()
        {
            using var correctionValuesType = H5Type.CreateDoubleArrayType(CorrectionBlobSize / CorrectionBlobTypeSize);

            int blobOffset = Marshal.SizeOf<SBladeProfileCalibration>();

            return H5Type
                .CreateCompoundType<SBladeProfileCalibration>(CorrectionBlobSize)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.Id), H5T.NATIVE_INT64)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.ProfileValue), H5T.NATIVE_DOUBLE)
                .Insert("CorrectionValues", blobOffset, correctionValuesType);
        }

        public override void Write(Action<IntPtr> write, IEnumerable<BladeProfileCalibration> inputRecords)
        {
            static SBladeProfileCalibration Convert(BladeProfileCalibration source)
            {
                return new SBladeProfileCalibration
                {
                    Id = source.Id,
                    BladeProfileCalibrationSetId = source.BladeProfileCalibrationSetId,
                    ProfileValue = source.ProfileValue
                };
            }

            var records = inputRecords.Select(Convert).ToList();
            var blobs = inputRecords.Select(r => new { r.Id, r.CorrectionValues }).ToList();

            // if any record.Value.Length != CorrectionBlobSize, throw
            var invalidLengths = blobs.Where(r => r.CorrectionValues.Length != CorrectionBlobSize).ToList();
            if(invalidLengths.Any())
            {
                throw new InvalidOperationException($"The Correction values length in BladeProfileCalibration for ids '{string.Join(",", invalidLengths.Select(l => l.Id))}' does not match the expected length of {CorrectionBlobSize}.");
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
                    using var pinnedBlob = new PinnedObject(blobs[i].CorrectionValues);

                    if(records[i].Id != blobs[i].Id)
                    {
                        throw new InvalidOperationException($"BladeProfileCalibration converter. Unexpected internal error. Ids not matching.");
                    }

                    // Copy SBladeProfileCalibration
                    Buffer.MemoryCopy(pinnedRecord, CurrentBufferPosition(), remainingBufferSize, structSize);
                    remainingBufferSize -= structSize;

                    // Copy correction values
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

        public static IH5TypeAdapter<BladeProfileCalibration> Create(int blobLength, int blobTypeSize) => 
            new BladeProfileCalibrationAdapter(blobLength, blobTypeSize);
    }
}
