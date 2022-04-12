using HDF.PInvoke;
using HDF5Api;
using HDF5Api.Disposables;
using PulseData.TvlAlt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="Waveform"/>.
    /// </summary>
    public sealed class WaveformAdapter : H5TypeAdapter<Waveform>
    {
        private readonly int WaveformBlobSize;
        private readonly int WaveformBlobTypeSize;

        private const int TypeLength = 9;

        private WaveformAdapter(int waveformBlobSize, int waveformBlobTypeSize)
        {
            AssertBlobLengthDivisibleByTypeLength(waveformBlobSize, waveformBlobTypeSize);

            WaveformBlobSize = waveformBlobSize;
            WaveformBlobTypeSize = waveformBlobTypeSize;
        }

        public override H5Type GetH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(WaveformBlobSize / WaveformBlobTypeSize);
            using var typeStringType = H5Type.CreateFixedLengthStringType(TypeLength);

            int valuesOffset = Marshal.SizeOf<SWaveform>();

            return H5Type
                .CreateCompoundType<SWaveform>(WaveformBlobSize)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.RecordId), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.Type), typeStringType)
                .Insert<SWaveform>(nameof(SWaveform.Offset), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.Spacing), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.ReferenceId), H5T.NATIVE_INT64)
                .Insert("Values", valuesOffset, valuesType);
        }

        public override void Write(Action<IntPtr> write, IEnumerable<Waveform> inputRecords)
        {
            static SWaveform Convert(Waveform source)
            {
                var result = new SWaveform
                {
                    Id = source.Id,
                    RecordId = source.RecordId,
                    Offset = source.Offset,
                    Spacing = source.Spacing,
                    ReferenceId = source.ReferenceId,
                };

                unsafe
                {
                    CopyString(source.Type, result.Type, TypeLength);
                }

                return result;
            }

            var records = inputRecords.Select(Convert).ToList();
            var blobs = inputRecords.Select(r => new { r.Id, r.Values }).ToList();

            // if any record.Value.Length != CorrectionBlobSize, throw
            var invalidLengths = blobs.Where(r => r.Values.Length != WaveformBlobSize).ToList();
            if (invalidLengths.Any())
            {
                throw new InvalidOperationException($"The Values length in Waveform for ids '{string.Join(",", invalidLengths.Select(l => l.Id))}' does not match the expected length of {WaveformBlobSize}.");
            }

            int structSize = Marshal.SizeOf<SWaveform>();
            int bufferSize = (structSize + WaveformBlobSize) * records.Count;

            using var buffer = new GlobalMemory(bufferSize);

            unsafe
            {
                var remainingBufferSize = bufferSize;

                for (int i = 0; i < records.Count; i++)
                {
                    using var pinnedRecord = new PinnedObject(records[i]);
                    using var pinnedBlob = new PinnedObject(blobs[i].Values);

                    if (records[i].Id != blobs[i].Id)
                    {
                        throw new InvalidOperationException($"Waveform converter. Unexpected internal error. Ids not matching.");
                    }

                    // Copy SWaveform
                    Buffer.MemoryCopy(pinnedRecord, CurrentBufferPosition(), remainingBufferSize, structSize);
                    remainingBufferSize -= structSize;

                    // Copy correction values
                    Buffer.MemoryCopy(pinnedBlob, CurrentBufferPosition(), remainingBufferSize, WaveformBlobSize);
                    remainingBufferSize -= WaveformBlobSize;
                }

                void* CurrentBufferPosition()
                {
                    return IntPtr.Add(buffer.IntPtr, bufferSize - remainingBufferSize).ToPointer();
                }
            }

            write(buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SWaveform
        {
            public long Id;
            public long RecordId;
            public double Offset;
            public double Spacing;
            public long ReferenceId;
            public fixed byte Type[TypeLength];
        }

        public static IH5TypeAdapter<Waveform> Create(int waveformBlobSize, int waveformBlobTypeSize) =>
            new WaveformAdapter(waveformBlobSize, waveformBlobTypeSize);
    }
}
