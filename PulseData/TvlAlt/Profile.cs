using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlAlt
{
    [Table("Profile")]
    public class Profile
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("RECORD_ID")]
        public long RecordId { get; set; }

        [ForeignKey("RecordId")]
        public virtual RawRecord RawRecord { get; set; }

        [Column("XZ_VALUES")]
        public byte[] Values { get; set; }

        [Column("UNITS")]
        [MaxLength(6)]
        public string Units { get; set; }
    }
}
