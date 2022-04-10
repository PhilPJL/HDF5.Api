using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="BladeProfileCalibrationSet"/>.
    /// </summary>
    public sealed class BladeProfileCalibrationSetAdapter : H5TypeAdapter<BladeProfileCalibrationSet, BladeProfileCalibrationSetAdapter.SBladeProfileCalibrationSet>
    {
        private const int DescriptionLength = 500;

        private BladeProfileCalibrationSetAdapter() { }

        protected override unsafe SBladeProfileCalibrationSet Convert(BladeProfileCalibrationSet source)
        {
            var result = new SBladeProfileCalibrationSet
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate(),
                DeltaFrequency = source.DeltaFrequency
            };

            unsafe
            {
                CopyString(source.Description, result.Description, DescriptionLength);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var descriptionStringType = H5Type.CreateFixedLengthStringType(DescriptionLength);

            return H5Type
                .CreateCompoundType<SBladeProfileCalibrationSet>()
                .Insert<SBladeProfileCalibrationSet>(nameof(SBladeProfileCalibrationSet.Id), H5T.NATIVE_INT64)
                .Insert<SBladeProfileCalibrationSet>(nameof(SBladeProfileCalibrationSet.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileCalibrationSet>(nameof(SBladeProfileCalibrationSet.DeltaFrequency), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileCalibrationSet>(nameof(SBladeProfileCalibrationSet.Description), descriptionStringType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeProfileCalibrationSet
        {
            public int Id;
            public double Timestamp;
            public double DeltaFrequency;
            public fixed byte Description[DescriptionLength];
        }

        public static IH5TypeAdapter<BladeProfileCalibrationSet> Default { get; } = new BladeProfileCalibrationSetAdapter();
    }
}
