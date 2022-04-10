using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="InstallationConfiguration"/>.
    /// </summary>
    public sealed class InstallationConfigurationAdapter : H5TypeAdapter<InstallationConfiguration, InstallationConfigurationAdapter.SInstallationConfiguration>
    {
        private const int ConfigurationLength = 12000;
        private const int IdentityLength = 1000;
        private const int SessionKeyLength = 32;
        private const int CommentLength = 2000;

        private InstallationConfigurationAdapter() { }

        protected override unsafe SInstallationConfiguration Convert(InstallationConfiguration source)
        {
            var result = new SInstallationConfiguration
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate()
            };

            CopyString(source.Configuration, result.Configuration, ConfigurationLength);
            CopyString(source.Identity, result.Identity, IdentityLength);
            CopyString(source.SessionKey, result.SessionKey, SessionKeyLength);
            CopyString(source.Comment, result.Comment, CommentLength);

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var configurationStringType = H5Type.CreateFixedLengthStringType(ConfigurationLength);
            using var identityStringType = H5Type.CreateFixedLengthStringType(IdentityLength);
            using var sessionKeyStringType = H5Type.CreateFixedLengthStringType(SessionKeyLength);
            using var commentStringType = H5Type.CreateFixedLengthStringType(CommentLength);

            return H5Type
                .CreateCompoundType<SInstallationConfiguration>()
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.Id), H5T.NATIVE_INT64)
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.Configuration), configurationStringType)
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.Identity), identityStringType)
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.SessionKey), sessionKeyStringType)
                .Insert<SInstallationConfiguration>(nameof(SInstallationConfiguration.Comment), commentStringType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SInstallationConfiguration
        {
            public long Id;
            public double Timestamp;
            public fixed byte Configuration[ConfigurationLength];
            public fixed byte Identity[IdentityLength];
            public fixed byte SessionKey[SessionKeyLength];
            public fixed byte Comment[CommentLength];
        }

        public static IH5TypeAdapter<InstallationConfiguration> Default { get; } = new InstallationConfigurationAdapter();
    }
}
