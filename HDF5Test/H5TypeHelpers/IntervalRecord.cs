using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class IntervalRecordHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SIntervalRecord
        {
            public long Id;
            public double Timestamp;
            public double AverageThickness;
            public double MinimumThickness;
            public double MaximumThickness;
        }

        public static H5Type CreateH5Type()
        {
            int size = Marshal.SizeOf<SIntervalRecord>();

            using var type = H5Type.CreateCompoundType(size);

            type.Insert<SIntervalRecord>(nameof(SIntervalRecord.Id), H5T.NATIVE_INT64);
            type.Insert<SIntervalRecord>(nameof(SIntervalRecord.Timestamp), H5T.NATIVE_DOUBLE);
            type.Insert<SIntervalRecord>(nameof(SIntervalRecord.AverageThickness), H5T.NATIVE_DOUBLE);
            type.Insert<SIntervalRecord>(nameof(SIntervalRecord.MinimumThickness), H5T.NATIVE_DOUBLE);
            type.Insert<SIntervalRecord>(nameof(SIntervalRecord.MaximumThickness), H5T.NATIVE_DOUBLE);

            return type;
        }

        public static SIntervalRecord Convert(IntervalRecord source)
        {
            return new SIntervalRecord
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate(),
                AverageThickness = source.AverageThickness ?? double.NaN,
                MinimumThickness = source.MinimumThickness ?? double.NaN,
                MaximumThickness = source.MaximumThickness ?? double.NaN,
            };
        }
    }
}
