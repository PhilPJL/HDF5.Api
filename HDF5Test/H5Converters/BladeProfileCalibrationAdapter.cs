using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="BladeProfileCalibration"/>.
    /// </summary>
    public sealed class BladeProfileCalibrationAdapter : H5TypeAdapter<BladeProfileCalibration, BladeProfileCalibrationAdapter.SBladeProfileCalibration>
    {
        private const int CorrectionBlobSize = 16384;
        private const int CorrectionBlobTypeSize = sizeof(double);
        private BladeProfileCalibrationAdapter() { }

        protected override unsafe SBladeProfileCalibration Convert(BladeProfileCalibration source)
        {
            var result = new SBladeProfileCalibration
            {
                Id = source.Id,
                BladeProfileCalibrationSetId = source.BladeProfileCalibrationSetId,
                ProfileValue = source.ProfileValue,
                CorrectionValuesLength = (source.Correction?.Length ?? 0) / CorrectionBlobTypeSize
            };

            unsafe
            {
                CopyBlob(source.Correction, result.CorrectionValues, CorrectionBlobSize, CorrectionBlobTypeSize);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var correctionValuesType = H5Type.CreateDoubleArrayType(CorrectionBlobSize / CorrectionBlobTypeSize);

            return H5Type
                .CreateCompoundType<SBladeProfileCalibration>()
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.Id), H5T.NATIVE_INT64)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.ProfileValue), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.CorrectionValuesLength), H5T.NATIVE_INT32)
                .Insert<SBladeProfileCalibration>(nameof(SBladeProfileCalibration.CorrectionValues), correctionValuesType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeProfileCalibration
        {
            public int Id;
            public double Timestamp;
            public int BladeProfileCalibrationSetId;
            public double ProfileValue;
            public int CorrectionValuesLength;
            public fixed byte CorrectionValues[CorrectionBlobSize];
        }

        public static IH5TypeAdapter<BladeProfileCalibration> Default { get; } = new BladeProfileCalibrationAdapter();
    }
}
