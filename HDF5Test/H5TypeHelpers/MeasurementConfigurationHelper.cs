using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class MeasurementConfigurationHelper
    {
        public const int nameLength = 6;
        public const int descriptionLength = 6;
        public const int moduleNameLength = 6;
        public const int scannerNameLength = 6;
        public const int scannerDescriptionLength = 6;
        public const int sessionKeyLength = 6;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SMeasurementConfiguration
        {
            public long Id;
            public fixed byte Name[nameLength];
            public fixed byte Description[descriptionLength];
            public fixed byte ModuleName[moduleNameLength];
            public fixed byte ScannerName[scannerNameLength];
            public fixed byte ScannerConfiguration[scannerDescriptionLength];
            public double Timestamp;
            public fixed byte SessionKey[sessionKeyLength];
        }

        public static H5Type CreateH5Type()
        {
            return H5Type
                .CreateCompoundType<SMeasurementConfiguration>()
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Id), H5T.NATIVE_INT64)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Name), H5T.NATIVE_UCHAR)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Description), H5T.NATIVE_UCHAR)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ModuleName), H5T.NATIVE_UCHAR)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ScannerName), H5T.NATIVE_UCHAR)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ScannerConfiguration), H5T.NATIVE_UCHAR)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.SessionKey), H5T.NATIVE_UCHAR);
        }


        public static SMeasurementConfiguration Convert(MeasurementConfiguration source)
        {
            return new SMeasurementConfiguration
            {
                Id = source.Id,
                //Name = source.Name,
                //Description = source.Description,   
                //ModuleName = source.ModuleName,
                //ScannerName = source.ScannerName,
                //ScannerConfiguration = source.ScannerConfiguration,
                //Timestamp = source.Timestamp.ToOADate(),
                //SessionKey = source.SessionKey
            };
        }
    }
}
