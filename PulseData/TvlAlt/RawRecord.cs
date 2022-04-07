using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlAlt
{
    [Table("RawRecord")]
    public class RawRecord
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("THICKNESS")]
        public long? Thickness { get; set; }

        [Column("PULSE_OFFSET")]
        public double? PulseOffset { get; set; }

        [Column("REFERENCE_OFFSET")]
        public double? ReferenceOffset { get; set; }

        [Column("PROFILE_DEVIATION")]
        public double? ProfileDeviation { get; set; }

        [Column("PROFILE_HEIGHT")]
        public double? ProfileHeight { get; set; }

        [Column("PROFILE_OFFSET")]
        public double? ProfileOffset { get; set; }

        [Column("Z_POSITION")]
        public double? ZPosition { get; set; }

        [Column("MEAS_ID")]
        public int MeasurementId { get; set; }

        [Column("INTERVAL_ID")]
        public long? IntervalId { get; set; }

        public virtual ICollection<Waveform> Waveforms { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }

        [ForeignKey("IntervalId")]
        public virtual IntervalRecord IntervalRecord { get; set; }
    }
}
