using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("InstConfig")]
    public class InstallationConfiguration
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("CONFIG")]
        [MaxLength(12000)]
        public string Configuration { get; set; }

        [Column("IDENTITY")]
        [MaxLength(1000)]
        public string Identity { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("SESSION_KEY")]
        [MaxLength(32)]
        public string SessionKey { get; set; }

        [Column("COMMENT")]
        [MaxLength(2000)]
        public string Comment { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }

        [ForeignKey("SessionKey")]
        public virtual Session Session { get; set; }
    }
}
