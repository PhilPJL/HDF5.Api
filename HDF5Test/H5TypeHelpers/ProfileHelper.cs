using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Test.H5TypeHelpers
{
    public class ProfileConverter : H5TypeConverterBase, IH5TypeConverter<Profile, ProfileConverter.SProfile>
    {
        public const int profileBlobSize = 32768;
        public const int unitsLength = 6;

        public unsafe SProfile Convert(Profile source)
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

            var msg = $"Profile: The provided units has length {source.Units.Length} which exceeds the maximum expected length {unitsLength}";

#if DEBUG
            if (source.Units.Length > unitsLength)
            {
                throw new InvalidOperationException(msg);
            }
#else
            System.Diagnostics.Trace.WriteLine(msg);
#endif

            byte[] bytes = Ascii.GetBytes(source.Units);

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0).ToPointer(), result.Units, unitsLength, bytes.Length);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), result.ValuesX, profileBlobSize / 2, pf.Length / 2);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, pf.Length / 2).ToPointer(), result.ValuesZ, profileBlobSize / 2, pf.Length / 2);

            return result;
        }

        public H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(profileBlobSize / sizeof(double) / 2);
            using var stringType = H5Type.CreateFixedLengthStringType(unitsLength);

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

            public fixed byte Units[unitsLength];

            public fixed byte ValuesX[profileBlobSize / 2];
            public fixed byte ValuesZ[profileBlobSize / 2];
        }

        public static IH5TypeConverter<Profile, SProfile> Default { get; } = new ProfileConverter();
    }

    public static class ProfileHelper
    {
        public const int profileBlobSize = 32768;
        public const int unitsLength = 6;
        private static readonly ASCIIEncoding Ascii = new();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SProfile
        {
            public long Id;
            public long RecordId;

            public fixed byte Units[unitsLength];

            public fixed byte ValuesX[profileBlobSize / 2];
            public fixed byte ValuesZ[profileBlobSize / 2];
        }

        public static H5Type CreateH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(profileBlobSize / sizeof(double) / 2);
            using var stringType = H5Type.CreateFixedLengthStringType(unitsLength);

            return H5Type
                .CreateCompoundType<SProfile>()
                .Insert<SProfile>(nameof(SProfile.Id), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.RecordId), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.Units), stringType)
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

            var msg = $"Profile: The provided units has length {source.Units.Length} which exceeds the maximum expected length {unitsLength}";

#if DEBUG
            if (source.Units.Length > unitsLength)
            {
                throw new InvalidOperationException(msg);
            }
#else
            System.Diagnostics.Trace.WriteLine(msg);
#endif

            byte[] bytes = Ascii.GetBytes(source.Units);

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0).ToPointer(), result.Units, unitsLength, bytes.Length);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, 0).ToPointer(), result.ValuesX, profileBlobSize / 2, pf.Length / 2);
            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(pf, pf.Length / 2).ToPointer(), result.ValuesZ, profileBlobSize / 2, pf.Length / 2);

            return result;
        }
    }
}
