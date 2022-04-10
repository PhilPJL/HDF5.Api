using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="BladeReference"/>.
    /// </summary>
    public sealed class BladeReferenceAdapter : H5TypeAdapter<BladeReference, BladeReferenceAdapter.SBladeReference>
    {
        private const int BladeWaveformBlobLength = 8;
        private const int BladeWaveformBlobTypeSize = sizeof(double);
        private const int MirrorWaveformBlobLength = 8;
        private const int MirrorWaveformBlobTypeSize = sizeof(double);

        private BladeReferenceAdapter() { }

        protected override SBladeReference Convert(BladeReference source)
        {
            var result = new SBladeReference
            {
                TOffset = source.TOffset,
                TSpacing = source.TSpacing,
                ProfileCurvature = source.ProfileCurvature,
                ProfileHeight = source.ProfileHeight,
                ProfileCentre = source.ProfileCentre,
                MirrorWaveformLength = (source.MirrorWaveform?.Length ?? 0)/ MirrorWaveformBlobTypeSize,
                BladeWaveformLength = (source.BladeWaveform?.Length ?? 0) / BladeWaveformBlobTypeSize
            };

            unsafe
            {
                CopyBlob(source.MirrorWaveform, result.MirrorWaveform, MirrorWaveformBlobLength, MirrorWaveformBlobTypeSize);
                CopyBlob(source.BladeWaveform, result.BladeWaveform, BladeWaveformBlobLength, BladeWaveformBlobTypeSize);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var mirrorWaveformType = H5Type.CreateDoubleArrayType(MirrorWaveformBlobLength / MirrorWaveformBlobTypeSize);
            using var bladeWaveformType = H5Type.CreateDoubleArrayType(BladeWaveformBlobLength / BladeWaveformBlobTypeSize);

            return H5Type
                .CreateCompoundType<SBladeReference>()
                .Insert<SBladeReference>(nameof(SBladeReference.TOffset), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.TSpacing), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileCurvature), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileHeight), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.ProfileCentre), H5T.NATIVE_DOUBLE)
                .Insert<SBladeReference>(nameof(SBladeReference.BladeWaveformLength), H5T.NATIVE_INT32)
                .Insert<SBladeReference>(nameof(SBladeReference.BladeWaveform), bladeWaveformType)
                .Insert<SBladeReference>(nameof(SBladeReference.MirrorWaveformLength), H5T.NATIVE_INT32)
                .Insert<SBladeReference>(nameof(SBladeReference.MirrorWaveform), mirrorWaveformType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SBladeReference
        {
            public double TOffset;
            public double TSpacing;
            public double ProfileCurvature;
            public double ProfileHeight;
            public double ProfileCentre;

            public int BladeWaveformLength;
            public fixed byte BladeWaveform[BladeWaveformBlobLength];

            public int MirrorWaveformLength;
            public fixed byte MirrorWaveform[MirrorWaveformBlobLength];
        }

        public static IH5TypeAdapter<BladeReference> Default { get; } = new BladeReferenceAdapter();
    }
}
