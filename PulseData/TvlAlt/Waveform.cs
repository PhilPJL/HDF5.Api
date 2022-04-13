using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlAlt
{
    [Table("Waveform")]
    public class Waveform
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("RECORD_ID")]
        public long RecordId { get; set; }

        [Column("TYPE")]
        [MaxLength(9)]
        public string Type { get; set; }

        [Column("T_OFFSET")]
        public double Offset { get; set; }

        [Column("T_SPACING")]
        public double Spacing { get; set; }

        [Column("REFERENCE_ID")]
        public long? ReferenceId { get; set; }

        [Column("WAVEFORM")]
        public byte[] Values { get; set; }

        [ForeignKey("RecordId")]
        public virtual RawRecord RawRecord { get; set; }
    }
}
