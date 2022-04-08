using HDF.PInvoke;
using HDF5Api;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class WaveformHelper
    {
        public const int WaveformBlobSize = 16384;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SWaveform
        {
            public long Id;
            public long RecordId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string Type;
            public double Offset;
            public double Spacing;
            public long ReferenceId;

            public fixed byte Values[WaveformBlobSize];
        }

        public static H5Type CreateH5Type()
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
    }
}
