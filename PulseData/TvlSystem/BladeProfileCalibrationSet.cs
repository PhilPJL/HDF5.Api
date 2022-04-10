using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("BladeProfCalibrationSet")]
    public class BladeProfileCalibrationSet
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("DELTA_FREQ")]
        public double DeltaFrequency { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        public virtual ICollection<BladeProfileCalibration> BladeProfileCalibrations { get; set; }
    }
}
