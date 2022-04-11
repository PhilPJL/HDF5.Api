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
    /// A type converter for <see cref="BladeReference"/>.
    /// </summary>
    public sealed class BladeReferenceVariableAdapter : H5TypeAdapter<BladeReference>
    {
        private readonly int BladeWaveformBlobLength;
        private readonly int BladeWaveformBlobTypeSize;
        private readonly int MirrorWaveformBlobLength;
        private readonly int MirrorWaveformBlobTypeSize;
        private const int DescriptionLength = 500;

        private BladeReferenceVariableAdapter(int bladeWaveformBlobLength, int bladeWaveformBlobTypeSize, int mirrorWaveformBlobLength, int mirrorWaveformBlobTypeSize)
        {
            AssertBlobLengthDivisibleByTypeLength(bladeWaveformBlobLength, bladeWaveformBlobTypeSize);
            AssertBlobLengthDivisibleByTypeLength(mirrorWaveformBlobLength, mirrorWaveformBlobTypeSize);

            BladeWaveformBlobLength = bladeWaveformBlobLength;
            BladeWaveformBlobTypeSize = bladeWaveformBlobTypeSize;
            MirrorWaveformBlobLength = mirrorWaveformBlobLength;
            MirrorWaveformBlobTypeSize = mirrorWaveformBlobTypeSize;
        }

        public override H5Type GetH5Type()
        {
            using var bladeWaveformType = H5Type.CreateDoubleArrayType(BladeWaveformBlobLength / BladeWaveformBlobTypeSize);
            using var mirrorWaveformType = H5Type.CreateDoubleArrayType(MirrorWaveformBlobLength / MirrorWaveformBlobTypeSize);
            using var descriptionStringType = H5Type.CreateFixedLengthStringType(DescriptionLength);

            int bladeWaveformBlobOffset = Marshal.SizeOf<SBladeReference>();
            int mirrorWaveformBlobOffset = bladeWaveformBlobOffset + BladeWaveformBlobLength;

            return H5Type
                .CreateCompoundType<SBladeReference>(BladeWaveformBlobLength + MirrorWaveformBlobLength)
                .Insert<SBladeReference>(nameof(SBladeReference.Id), H5T.NATIVE_INT32)
                .Insert<SBladeReference>(nameof(SBladeReference.TOffset), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.TSpacing), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileCurvature), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileHeight), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileCentre), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.Description), descriptionStringType)
                .Insert("BladeWaveform", bladeWaveformBlobOffset, bladeWaveformType)
                .Insert("MirrorWaveform", mirrorWaveformBlobOffset, mirrorWaveformType);
        }

        public override void WriteChunk(Action<IntPtr> write, IEnumerable<BladeReference> inputRecords)
        {
            SBladeReference Convert(BladeReference source)
            {
                var result = new SBladeReference
                {
                    Id = source.Id,
                    TOffset = source.TOffset,
                    TSpacing = source.TSpacing,
                    ProfileCurvature = source.ProfileCurvature,
                    ProfileHeight = source.ProfileHeight,
                    ProfileCentre = source.ProfileCentre,
                };

                unsafe
                {
                    CopyString(source.Description, result.Description, DescriptionLength);
                }

                return result;
            }

            var records = inputRecords.Select(Convert).ToList();
            var blobs = inputRecords.Select(r => new { r.Id, r.MirrorWaveform, r.BladeWaveform }).ToList();

            // TODO: make assertions
            var invalidLengths = blobs.Where(r => r.MirrorWaveform.Length != MirrorWaveformBlobLength).ToList();
            if (invalidLengths.Any())
            {
                throw new InvalidOperationException($"The MirrorWaveform values length in BladeReferenceVariable for ids '{string.Join(",", invalidLengths.Select(l => l.Id))}' do not match the expected length of {MirrorWaveformBlobLength}.");
            }

            var invalidLengths2 = blobs.Where(r => r.BladeWaveform.Length != BladeWaveformBlobLength).ToList();
            if (invalidLengths2.Any())
            {
                throw new InvalidOperationException($"The BladeWaveform values length in BladeReferenceVariable for ids '{string.Join(",", invalidLengths2.Select(l => l.Id))}' do not match the expected length of {BladeWaveformBlobLength}.");
            }

            int structSize = Marshal.SizeOf<SBladeReference>();
            int bufferSize = (structSize + MirrorWaveformBlobLength + BladeWaveformBlobLength) * records.Count;

            using var buffer = new GlobalMemory(bufferSize);

            unsafe
            {
                var remainingBufferSize = bufferSize;

                for (int i = 0; i < records.Count; i++)
                {
                    using var pinnedRecord = new PinnedObject(records[i]);
                    using var pinnedBladeWaveformBlob = new PinnedObject(blobs[i].BladeWaveform);
                    using var pinnedMirrorWaveformBlob = new PinnedObject(blobs[i].MirrorWaveform);

                    if (records[i].Id != blobs[i].Id)
                    {
                        throw new InvalidOperationException($"BladeReference converter. Unexpected internal error. Ids not matching.");
                    }

                    // Copy SBladeReference
                    Buffer.MemoryCopy(pinnedRecord, CurrentBufferPosition(), remainingBufferSize, structSize);
                    remainingBufferSize -= structSize;

                    // Copy blade waveform
                    Buffer.MemoryCopy(pinnedBladeWaveformBlob, CurrentBufferPosition(), remainingBufferSize, BladeWaveformBlobLength);
                    remainingBufferSize -= BladeWaveformBlobLength;

                    // Copy mirror waveform
                    Buffer.MemoryCopy(pinnedMirrorWaveformBlob, CurrentBufferPosition(), remainingBufferSize, MirrorWaveformBlobLength);
                    remainingBufferSize -= MirrorWaveformBlobLength;
                }

                void* CurrentBufferPosition()
                {
                    return IntPtr.Add(buffer.IntPtr, bufferSize - remainingBufferSize).ToPointer();
                }
            }

            write(buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeReference
        {
            public int Id;
            public double TOffset;
            public double TSpacing;
            public double ProfileCurvature;
            public double ProfileHeight;
            public double ProfileCentre;
            public fixed byte Description[DescriptionLength];
        }

        public static IH5TypeAdapter<BladeReference> Create(int bladeWaveformBlobLength, int bladeWaveformblobTypeSize, int mirrorWaveformBlobLength, int mirrorWaveformblobTypeSize)
            => new BladeReferenceVariableAdapter(bladeWaveformBlobLength, bladeWaveformblobTypeSize, mirrorWaveformBlobLength, mirrorWaveformblobTypeSize);
    }
}
