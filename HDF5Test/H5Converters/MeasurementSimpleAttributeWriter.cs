using HDF5Api;
using PulseData.TvlSystem;

namespace HDF5Test.H5TypeHelpers
{
    public static class MeasurementSimpleAttributeWriter
    {
        private const string Prefix = "Measurement:";

        public static void WriteAttributes(IH5Location location, Measurement configuration)
        {
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.Id), configuration.Id);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.Timestamp), configuration.Timestamp);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.SessionKey), configuration.SessionKey);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.MeasurementConfigurationId), configuration.MeasurementConfigurationId);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.InstallationConfigurationId), configuration.InstallationConfigurationId);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.Status), configuration.Status);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.Geometry), configuration.Geometry);
            location.CreateAndWriteAttribute(Prefix + nameof(Measurement.Comment), configuration.Comment);
        }
    }
}
