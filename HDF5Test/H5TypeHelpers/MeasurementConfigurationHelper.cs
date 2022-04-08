using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    public static class MeasurementConfigurationHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMeasurementConfiguration
        {
            public long Id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string ModuleName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string ScannerName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
            public string ScannerConfiguration;
            public double Timestamp;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string SessionKey;
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
                Name = source.Name,
                Description = source.Description,   
                ModuleName = source.ModuleName,
                ScannerName = source.ScannerName,
                ScannerConfiguration = source.ScannerConfiguration,
                Timestamp = source.Timestamp.ToOADate(),
                SessionKey = source.SessionKey
            };
        }
    }
}
