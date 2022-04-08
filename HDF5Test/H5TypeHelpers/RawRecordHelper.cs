using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class RawRecordHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SRawRecord
        {
            public long Id;
            public int MeasurementId;
            public double Timestamp;
            public double Thickness;
            public double ProfileDeviation;
            public double ProfileHeight;
            public double ZPosition;
            public long IntervalId;
            public double PulseOffset;
            public double ReferenceOffset;
        }

        public static H5Type CreateH5Type()
        {
            int size = Marshal.SizeOf<SRawRecord>();

            using var type = H5Type.CreateCompoundType(size);

            type.Insert<SRawRecord>(nameof(SRawRecord.Id), H5T.NATIVE_INT64);
            type.Insert<SRawRecord>(nameof(SRawRecord.MeasurementId), H5T.NATIVE_INT32);
            type.Insert<SRawRecord>(nameof(SRawRecord.Timestamp), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.Thickness), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.ProfileDeviation), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.ProfileHeight), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.ZPosition), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.IntervalId), H5T.NATIVE_INT64);
            type.Insert<SRawRecord>(nameof(SRawRecord.PulseOffset), H5T.NATIVE_DOUBLE);
            type.Insert<SRawRecord>(nameof(SRawRecord.ReferenceOffset), H5T.NATIVE_DOUBLE);

            return type;
        }

        public static SRawRecord Convert(RawRecord source)
        {
            return new SRawRecord
            {
                Id = source.Id,
                MeasurementId = source.MeasurementId,
                Timestamp = source.Timestamp.ToOADate(),
                Thickness = source.Thickness ?? double.NaN,
                IntervalId = source.IntervalId ?? 0,
                ProfileDeviation = source.ProfileDeviation ?? double.NaN,
                ProfileHeight = source.ProfileHeight ?? double.NaN,
                ZPosition = source.ZPosition ?? double.NaN,
                PulseOffset = source.PulseOffset ?? double.NaN,
                ReferenceOffset = source.ReferenceOffset ?? double.NaN
            };
        }
    }
}
