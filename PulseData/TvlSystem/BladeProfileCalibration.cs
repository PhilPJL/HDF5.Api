using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("BladeProfCalibration")]
    public class BladeProfileCalibration
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("SET_ID")]
        public int BladeProfileCalibrationSetId { get; set; }

        [ForeignKey("BladeProfileCalibrationSetId")]
        public virtual BladeProfileCalibrationSet BladeProfileCalibrationSet { get; set; }

        [Column("PROFILE_VALUE")]
        public double ProfileValue { get; set; }

        [Column("CORRECTION")]
        public byte[] Correction { get; set; }
    }
}
