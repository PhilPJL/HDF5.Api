using HDF5Api;
using PulseData.TvlSystem;

namespace HDF5Test.H5TypeHelpers
{
    public static class MeasurementConfigurationSimpleAttributeWriter
    {
        private const string Prefix = "MeasurementConfig:";

        public static void WriteAttributes(IH5Location location, MeasurementConfiguration configuration)
        {
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.Id), configuration.Id);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.Timestamp), configuration.Timestamp);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.Name), configuration.Name);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.Description), configuration.Description);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.ModuleName), configuration.ModuleName);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.ScannerName), configuration.ScannerName);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.SessionKey), configuration.SessionKey);
            location.CreateAndWriteAttribute(Prefix + nameof(MeasurementConfiguration.ScannerConfiguration), configuration.ScannerConfiguration);
        }
    }
}
