using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public class WaveformConverter : H5TypeConverterBase, IH5TypeConverter<Waveform, WaveformConverter.SWaveform>
    {
        private const int WaveformBlobSize = 16384;
        private const int TypeLength = 9;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SWaveform
        {
            public long Id;
            public long RecordId;
            public double Offset;
            public double Spacing;
            public long ReferenceId;

            public fixed byte Type[TypeLength];
            public fixed byte Values[WaveformBlobSize];
        }

        public H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(WaveformBlobSize / sizeof(double));

            return H5Type
                .CreateCompoundType<SWaveform>()
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_UCHAR)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_DOUBLE)
                .Insert<SWaveform>(nameof(SWaveform.Id), H5T.NATIVE_INT64)
                .Insert<SWaveform>(nameof(SWaveform.Values), valuesType);
        }

        public SWaveform Convert(Waveform source)
        {
            throw new System.NotImplementedException();
        }

        public static IH5TypeConverter<Waveform, SWaveform> Default { get; } = new WaveformConverter();
    }
}
