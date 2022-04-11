using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="Measurement"/>.
    /// </summary>
    public sealed class MeasurementAdapter : H5TypeAdapter<Measurement, MeasurementAdapter.SMeasurement>
    {
        private const int SessionKeyLength = 32;
        private const int StatusLength = 11;
        private const int GeometryLength = 12;
        private const int CommentLength = 8000;

        private MeasurementAdapter() { }

        protected override SMeasurement Convert(Measurement source)
        {
            var result = new SMeasurement
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate(),
                InstallationConfigurationId = source.InstallationConfigurationId,
                MeasurementConfigurationId = source.MeasurementConfigurationId
            };

            unsafe
            {
                CopyString(source.Comment, result.Comment, CommentLength);
                CopyString(source.Status, result.Status, StatusLength);
                CopyString(source.Geometry, result.Geometry, GeometryLength);
                CopyString(source.SessionKey, result.SessionKey, SessionKeyLength);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var sessionKeyStringType = H5Type.CreateFixedLengthStringType(SessionKeyLength);
            using var statusStringType = H5Type.CreateFixedLengthStringType(StatusLength);
            using var commentStringType = H5Type.CreateFixedLengthStringType(CommentLength);
            using var geometryStringType = H5Type.CreateFixedLengthStringType(GeometryLength);

            return H5Type
                .CreateCompoundType<SMeasurement>()
                .Insert<SMeasurement>(nameof(SMeasurement.Id), H5T.NATIVE_INT64)
                .Insert<SMeasurement>(nameof(SMeasurement.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SMeasurement>(nameof(SMeasurement.InstallationConfigurationId), H5T.NATIVE_INT32)
                .Insert<SMeasurement>(nameof(SMeasurement.MeasurementConfigurationId), H5T.NATIVE_INT32)
                .Insert<SMeasurement>(nameof(SMeasurement.Comment), commentStringType)
                .Insert<SMeasurement>(nameof(SMeasurement.Status), statusStringType)
                .Insert<SMeasurement>(nameof(SMeasurement.Geometry), geometryStringType)
                .Insert<SMeasurement>(nameof(SMeasurement.SessionKey), sessionKeyStringType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SMeasurement
        {
            public long Id;
            public double Timestamp;
            public fixed byte SessionKey[SessionKeyLength];
            public int MeasurementConfigurationId;
            public int InstallationConfigurationId;

            public fixed byte Status[StatusLength];
            public fixed byte Geometry[GeometryLength];
            public fixed byte Comment[CommentLength];
        }

        public static IH5TypeAdapter<Measurement> Default { get; } = new MeasurementAdapter();
    }
}
