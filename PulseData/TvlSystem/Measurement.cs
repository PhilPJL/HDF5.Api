using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("Measurement")]
    public class Measurement
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("SESSION_KEY")]
        public string SessionKey { get; set; }

        [Column("MEAS_CONFIG_ID")]
        public int MeasurementConfigurationId { get; set; }

        [ForeignKey("MeasurementConfigurationId")]
        public virtual MeasurementConfiguration MeasurementConfiguration { get; set; }

        [Column("INST_CONFIG_ID")]
        public int InstallationConfigurationId { get; set; }

        [ForeignKey("InstallationConfigurationId")]
        public virtual InstallationConfiguration InstallationConfiguration { get; set; }

        [Column("COMMENT")]
        public string Comment { get; set; }

        [Column("STATUS")]
        public string Status { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("GEOMETRY")]
        public string Geometry { get; set; }

        [ForeignKey("SessionKey")]
        public virtual Session Session { get; set; }
    }
}
