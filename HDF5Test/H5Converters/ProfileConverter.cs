﻿using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="Profile"/>.
    /// </summary>
    public sealed class ProfileAdapter : H5TypeAdapter<Profile, ProfileAdapter.SProfile>
    {
        public const int ProfileBlobSize = 32768;
        public const int UnitsLength = 6;

        private ProfileAdapter() { }

        protected override unsafe SProfile Convert(Profile source)
        {
            var result = new SProfile()
            {
                Id = source.Id,
                RecordId = source.RecordId,
            };

            CopyString(source.Units, result.Units, UnitsLength);

            result.ValuesLength = source.Values.Length / 2;

            unsafe
            {
                var span = new Span<byte>(source.Values);
                CopyBlob(span.Slice(0, span.Length / 2).ToArray(), result.ValuesX, ProfileBlobSize / 2, sizeof(double));
                CopyBlob(span.Slice(span.Length / 2, span.Length / 2).ToArray(), result.ValuesZ, ProfileBlobSize / 2, sizeof(double));
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var valuesType = H5Type.CreateDoubleArrayType(ProfileBlobSize / sizeof(double) / 2);
            using var stringType = H5Type.CreateFixedLengthStringType(UnitsLength);

            return H5Type
                .CreateCompoundType<SProfile>()
                .Insert<SProfile>(nameof(SProfile.Id), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.RecordId), H5T.NATIVE_INT64)
                .Insert<SProfile>(nameof(SProfile.Units), stringType)
                .Insert<SProfile>(nameof(SProfile.ValuesLength), H5T.NATIVE_INT32)
                .Insert<SProfile>(nameof(SProfile.ValuesX), valuesType)
                .Insert<SProfile>(nameof(SProfile.ValuesZ), valuesType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SProfile
        {
            public long Id;
            public long RecordId;

            public fixed byte Units[UnitsLength];
            public int ValuesLength;
            public fixed byte ValuesX[ProfileBlobSize / 2];
            public fixed byte ValuesZ[ProfileBlobSize / 2];
        }

        public static IH5TypeAdapter<Profile> Default { get; } = new ProfileAdapter();
    }
}
