﻿using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public class WaveformConverter : H5TypeConverterBase, IH5TypeConverter<Waveform, WaveformConverter.SWaveform>
    {
        private const int WaveformBlobSize = 16384;
        private const int TypeLength = 9;

        public unsafe SWaveform Convert(Waveform source)
        {
            var result = new SWaveform
            {
                Id = source.Id,
                RecordId = source.RecordId,
                Offset = source.Offset,
                Spacing = source.Spacing,
                ReferenceId = source.ReferenceId,
                ValuesLength = source.Values.Length / sizeof(double)
            };

            // Convert to assertions
            var pf = source.Values;

            // TODO: add assertion
            // TODO: check is multiple of sizeof(double)
            if (pf.Length > WaveformBlobSize)
            {
                throw new InvalidOperationException($"Waveform: The provided data blob is length {pf.Length} which exceeds the maximum expected length {WaveformBlobSize}");
            }

            CopyString(source.Type, result.Type, TypeLength);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), result.Values, WaveformBlobSize, pf.Length);

            return result;
        }

        public H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(WaveformBlobSize / sizeof(double));
            using var typeStringType = H5Type.CreateFixedLengthStringType(TypeLength);

            return H5Type
                .CreateCompoundType<SWaveform>()
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.RecordId), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.Type), typeStringType)
                .Insert<SWaveform>(nameof(SWaveform.Offset), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.Spacing), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.ReferenceId), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.ValuesLength), H5T.NATIVE_INT32)
                .Insert<SWaveform>(nameof(SWaveform.Values), valuesType);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SWaveform
        {
            public long Id;
            public long RecordId;
            public double Offset;
            public double Spacing;
            public long ReferenceId;

            public fixed byte Type[TypeLength];

            public int ValuesLength;
            public fixed byte Values[WaveformBlobSize];
        }

        public static IH5TypeConverter<Waveform, SWaveform> Default { get; } = new WaveformConverter();
    }
}
