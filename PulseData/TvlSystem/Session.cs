using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("Session")]
    public class Session
    {
        [Key]
        [Column("SESSION_KEY")]
        public string SessionKey { get; set; }

        [Column("SESSION_TYPE")]
        public string SessionType { get; set; }

        [Column("USER_ID")]
        public int UserId { get; set; }

        [Column("START_TIME")]
        public DateTime StartTime { get; set; }

        [Column("UPDATE_TIME")]
        public DateTime UpdateTime { get; set; }

        [Column("EXPIRED")]
        public bool Expired { get; set; }

        public virtual ICollection<InstallationConfiguration> InstallationConfigurations { get; set; }
        public virtual ICollection<MeasurementConfiguration> MeasurementConfigurations { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
    }
}
