using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public class ProfileConverter : H5TypeConverterBase, IH5TypeConverter<Profile, ProfileConverter.SProfile>
    {
        public const int ProfileBlobSize = 32768;
        public const int UnitsLength = 6;

        public unsafe SProfile Convert(Profile source)
        {
            var result = new SProfile()
            {
                Id = source.Id,
                RecordId = source.RecordId,
            };

            // Convert to assertions
            var pf = source.Values;

            // TODO: add assertion
            if (pf.Length > ProfileBlobSize)
            {
                throw new InvalidOperationException($"Profile: The provided data blob is length {pf.Length} which exceeds the maximum expected length {ProfileBlobSize}");
            }

            CopyString(source.Units, result.Units, UnitsLength);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), result.ValuesX, ProfileBlobSize / 2, pf.Length / 2);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, pf.Length / 2).ToPointer(), result.ValuesZ, ProfileBlobSize / 2, pf.Length / 2);

            return result;
        }

        public H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(ProfileBlobSize / sizeof(double) / 2);
            using var stringType = H5Type.CreateFixedLengthStringType(UnitsLength);

            return H5Type
                .CreateCompoundType<SProfile>()
                .Insert<SProfile>(nameof(SProfile.Id), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.RecordId), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.Units), stringType)
                .Insert<SProfile>(nameof(SProfile.ValuesX), valuesType)
                .Insert<SProfile>(nameof(SProfile.ValuesZ), valuesType);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SProfile
        {
            public long Id;
            public long RecordId;

            public fixed byte Units[UnitsLength];
            public fixed byte ValuesX[ProfileBlobSize / 2];
            public fixed byte ValuesZ[ProfileBlobSize / 2];
        }

        public static IH5TypeConverter<Profile, SProfile> Default { get; } = new ProfileConverter();
    }
}
