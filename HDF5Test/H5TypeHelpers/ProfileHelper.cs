using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class ProfileHelper
    {
        public const int profileBlobSize = 32768;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SProfile
        {
            public SProfile()
            {
            }

            public long Id = 0;
            public long RecordId = 0;

            public fixed char Units[6];

            public fixed byte ValuesX[profileBlobSize / 2];
            public fixed byte ValuesZ[profileBlobSize / 2];
        }

        public static H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(profileBlobSize / sizeof(double) / 2);

            return H5Type
                .CreateCompoundType<SProfile>()
                .Insert<SProfile>(nameof(SProfile.Id), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.RecordId), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.Units), H5T.NATIVE_UCHAR)
                .Insert<SProfile>(nameof(SProfile.ValuesX), valuesType)
                .Insert<SProfile>(nameof(SProfile.ValuesZ), valuesType);
        }

        public unsafe static SProfile Convert(Profile source)
        {
            var result = new SProfile()
            {
                Id = source.Id,
                RecordId = source.RecordId,
            };

            var pf = source.Values;

            if (pf.Length > profileBlobSize)
            {
                throw new InvalidOperationException($"Profile: The provided data blob is length {pf.Length} which exceeds the maximum expected length {profileBlobSize}");
            }

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(source.Units.ToCharArray(), 0).ToPointer(), result.Units, sizeof(char) * 6, source.Units.Length * 2);

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), result.ValuesX, profileBlobSize / 2, pf.Length / 2);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, pf.Length / 2).ToPointer(), result.ValuesZ, profileBlobSize / 2, pf.Length / 2);

            return result;
        }
    }
}
