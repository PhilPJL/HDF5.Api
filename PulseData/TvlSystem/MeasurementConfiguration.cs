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
        public string Name { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("module_name")]
        public string ModuleName { get; set; }

        [Column("SCANNER_NAME")]
        public string ScannerName { get; set; }

        [Column("SCANNER_CONFIG")]
        public string ScannerConfiguration { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("SESSION_KEY")]
        public string SessionKey { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }

        [ForeignKey("SessionKey")]
        public virtual Session Session { get; set; }
    }
}
