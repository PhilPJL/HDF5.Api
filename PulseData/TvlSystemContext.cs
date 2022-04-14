using EntityFramework.Firebird;
using EntityFramework.Functions;
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
#if DEBUG
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new FunctionConvention(typeof(FirebirdFunctions)));
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
