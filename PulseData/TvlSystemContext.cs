using EntityFramework.Firebird;
using PulseData.TvlSystem;
using System.Data.Common;
using System.Data.Entity;

namespace PulseData
{
    public class TvlSystemContext : DbContext
    {
        private static DbConnection GetConnection(string nameOrConnectionString)
        {
            return new FbConnectionFactory().CreateConnection(nameOrConnectionString);
        }

        public TvlSystemContext(string nameOrConnectionString = "TvlSystem") : base(GetConnection(nameOrConnectionString), true)
        {
            Database.SetInitializer<TvlSystemContext>(null);
        }

        public DbSet<BladeProfileName> BladeProfileNames { get; set; }
        public DbSet<ProfilerCalibration> ProfilerCalibrations { get; set; }
        public DbSet<BladeReference> BladeReferences { get; set; }
        public DbSet<BladeProfileCalibration> BladeProfileCalibrations { get; set; }
        public DbSet<BladeProfileCalibrationSet> BladeProfileCalibrationSets { get; set; }
        public DbSet<InstallationConfiguration> InstallationConfigurations { get; set; }
        public DbSet<MeasurementConfiguration> MeasurementConfigurations { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
