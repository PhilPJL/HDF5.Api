using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("MeasConfig")]
    public class MeasurementConfiguration
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("NAME")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Column("DESCRIPTION")]
        [MaxLength(255)]
        public string Description { get; set; }

        [Column("module_name")]
        [MaxLength(255)]
        public string ModuleName { get; set; }

        [Column("SCANNER_NAME")]
        [MaxLength(255)]
        public string ScannerName { get; set; }

        [Column("SCANNER_CONFIG")]
        [MaxLength(8000)]
        public string ScannerConfiguration { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("SESSION_KEY")]
        [MaxLength(32)]
        public string SessionKey { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }

        [ForeignKey("SessionKey")]
        public virtual Session Session { get; set; }
    }
}
