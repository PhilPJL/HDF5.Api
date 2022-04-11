using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="BladeProfileName"/>.
    /// </summary>
    public sealed class BladeProfileNameAdapter : H5TypeAdapter<BladeProfileName, BladeProfileNameAdapter.SBladeProfileName>
    {
        private const int NameLength = 50;
        private const int DescriptionLength = 500;

        private BladeProfileNameAdapter() { }

        protected override SBladeProfileName Convert(BladeProfileName source)
        {
            var result = new SBladeProfileName
            {
                ProfileCalibrationId = source.ProfilerCalibrationId ?? 0,
                BladeReferenceId = source.BladeReferenceId ?? 0,
                HeightCalibrationId = source.HeightCalibrationId ?? 0,
                AlgorithmType = source.AlgorithmType,
                Scaling0 = source.Scaling0,
                Scaling1 = source.Scaling1,
                Scaling2 = source.Scaling2,
                Scaling3 = source.Scaling3
            };

            unsafe
            {
                CopyString(source.Name, result.Name, NameLength);
                CopyString(source.Description, result.Description, DescriptionLength);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var nameStringType = H5Type.CreateFixedLengthStringType(NameLength);
            using var descriptionStringType = H5Type.CreateFixedLengthStringType(DescriptionLength);

            return H5Type
                .CreateCompoundType<SBladeProfileName>()
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Name), nameStringType)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Description), descriptionStringType)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.ProfileCalibrationId), H5T.NATIVE_INT32)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.BladeReferenceId), H5T.NATIVE_INT32)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.HeightCalibrationId), H5T.NATIVE_INT32)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.AlgorithmType), H5T.NATIVE_INT32)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Scaling0), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Scaling1), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Scaling2), H5T.NATIVE_DOUBLE)
                .Insert<SBladeProfileName>(nameof(SBladeProfileName.Scaling3), H5T.NATIVE_DOUBLE);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeProfileName
        {
            public fixed byte Name[NameLength];
            public fixed byte Description[DescriptionLength];

            public int ProfileCalibrationId;
            public int BladeReferenceId;
            public int HeightCalibrationId;
            public int AlgorithmType;

            public double Scaling0;
            public double Scaling1;
            public double Scaling2;
            public double Scaling3;
        }

        public static IH5TypeAdapter<BladeProfileName> Default { get; } = new BladeProfileNameAdapter();
    }
}
