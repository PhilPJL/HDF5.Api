using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public class MeasurementConfigurationConverter : H5TypeConverterBase, IH5TypeConverter<MeasurementConfiguration, MeasurementConfigurationConverter.SMeasurementConfiguration>
    {
        private const int NameLength = 50;
        private const int DescriptionLength = 255;
        private const int ModuleNameLength = 255;
        private const int ScannerNameLength = 255;
        private const int ScannerConfigurationLength = 8000;
        private const int SessionKeyLength = 32;

        public H5Type CreateH5Type()
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

        public SMeasurementConfiguration Convert(MeasurementConfiguration source)
        {
            return new SMeasurementConfiguration
            {
                Id = source.Id,
                // TODL copy strings
                //Name = source.Name,
                //Description = source.Description,   
                //ModuleName = source.ModuleName,
                //ScannerName = source.ScannerName,
                //ScannerConfiguration = source.ScannerConfiguration,
                //Timestamp = source.Timestamp.ToOADate(),
                //SessionKey = source.SessionKey
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SMeasurementConfiguration
        {
            public long Id;
            public fixed byte Name[NameLength];
            public fixed byte Description[DescriptionLength];
            public fixed byte ModuleName[ModuleNameLength];
            public fixed byte ScannerName[ScannerNameLength];
            public fixed byte ScannerConfiguration[ScannerConfigurationLength];
            public double Timestamp;
            public fixed byte SessionKey[SessionKeyLength];
        }

        public static IH5TypeConverter<MeasurementConfiguration, SMeasurementConfiguration> Default { get; } = new MeasurementConfigurationConverter();
    }
}
