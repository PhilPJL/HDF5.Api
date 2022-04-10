using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="Waveform"/>.
    /// </summary>
    public sealed class WaveformAdapter : H5TypeAdapter<Waveform, WaveformAdapter.SWaveform>
    {
        private const int WaveformBlobSize = 16384;
        private const int WaveformBlobTypeSize = sizeof(double);

        private const int TypeLength = 9;

        private WaveformAdapter() { }

        protected override SWaveform Convert(Waveform source)
        {
            var result = new SWaveform
            {
                Id = source.Id,
                RecordId = source.RecordId,
                Offset = source.Offset,
                Spacing = source.Spacing,
                ReferenceId = source.ReferenceId,
                ValuesLength = (source.Values?.Length ?? 0) / WaveformBlobTypeSize
            };

            unsafe
            {
                CopyString(source.Type, result.Type, TypeLength);
                CopyBlob(source.Values, result.Values, WaveformBlobSize, WaveformBlobTypeSize);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(WaveformBlobSize / WaveformBlobTypeSize);
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

        [StructLayout(LayoutKind.Sequential)]
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

        public static IH5TypeAdapter<Waveform> Default { get; } = new WaveformAdapter();
    }
}
