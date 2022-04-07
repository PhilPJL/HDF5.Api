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
        public string Configuration { get; set; }

        [Column("IDENTITY")]
        public string Identity { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("SESSION_KEY")]
        public string SessionKey { get; set; }

        [Column("COMMENT")]
        public string Comment { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }

        [ForeignKey("SessionKey")]
        public virtual Session Session { get; set; }
    }
}
