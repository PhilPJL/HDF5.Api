using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("BladeProfNames")]
    public class BladeProfileName
    {
        [Key]
        [Column("NAME")]
        public string Name { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("PROFILE_CAL_ID")]
        public int? ProfilerCalibrationId { get; set; }

        [ForeignKey("ProfilerCalibrationId")]
        public ProfilerCalibration ProfilerCalibration { get; set; }

        [Column("BLADE_REF_ID")]
        public int? BladeReferenceId { get; set; }

        [ForeignKey("BladeReferenceId")]
        public BladeReference BladeReference { get; set; }

        [Column("HEIGHT_CAL_ID")]
        public int? HeightCalibrationId { get; set; }

        // HeightCalibration ?

        [Column("ALGORITHM_TYPE")]
        public int AlgorithmType { get; set; }

        [Column("SCALING_0")]
        public double Scaling0 { get; set; }

        [Column("SCALING_1")]
        public double Scaling1 { get; set; }

        [Column("SCALING_2")]
        public double Scaling2 { get; set; }

        [Column("SCALING_3")]
        public double Scaling3 { get; set; }
    }
}
