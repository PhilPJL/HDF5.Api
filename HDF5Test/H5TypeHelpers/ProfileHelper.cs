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
            public long Id;
            public long RecordId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string Units;

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

        public static SProfile Convert(Profile source)
        {
            var result = new SProfile()
            {
                Id = source.Id,
                Units = source.Units,
                RecordId = source.RecordId,
            };

            // TODO: values
            throw new NotImplementedException("");

            return result;
        }
    }
}
