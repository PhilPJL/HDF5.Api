using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlAlt
{
    [Table("IntervalRecord")]
    public class IntervalRecord
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("AVE_THICKNESS")]
        public double? AverageThickness { get; set; }

        [Column("MIN_THICKNESS")]
        public double? MinimumThickness { get; set; }

        [Column("MAX_THICKNESS")]
        public double? MaximumThickness { get; set; }

        public virtual ICollection<RawRecord> RawRecords { get; set; }
    }
}
