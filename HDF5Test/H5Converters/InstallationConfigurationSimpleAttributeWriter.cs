using HDF5Api;
using PulseData.TvlSystem;

namespace HDF5Test.H5TypeHelpers
{
    public static class InstallationConfigurationSimpleAttributeWriter
    {
        private const string Prefix = "InstallationConfig:";

        public static void WriteAttributes(IH5Location location, InstallationConfiguration configuration)
        {
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.Id), configuration.Id);
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.Timestamp), configuration.Timestamp);
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.SessionKey), configuration.SessionKey);
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.Identity), configuration.Identity);
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.Comment), configuration.Comment);
            location.CreateAndWriteAttribute(Prefix + nameof(InstallationConfiguration.Configuration), configuration.Configuration);
        }
    }
}
