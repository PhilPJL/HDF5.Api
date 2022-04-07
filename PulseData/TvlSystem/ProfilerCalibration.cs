using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("ProfilerCalibration")]
    public class ProfilerCalibration
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("MEAS_ID")]
        public int MeasurementId { get; set; }

        [Column("PROFILE_DATA_A")]
        public byte[] ProfileDataA { get; set; }

        [Column("PROFILE_DATA_B")]
        public byte[] ProfileDataB { get; set; }

        [Column("PROFILE_DATA_C")]
        public byte[] ProfileDataC { get; set; }

        [Column("PROFILE_DATA_D")]
        public byte[] ProfileDataD { get; set; }

        [Column("TRANSFORM")]
        public byte[] Transform { get; set; }

        [Column("ERROR_REPORT")]
        public string ErrorReport { get; set; }

        public virtual ICollection<BladeProfileName> BladeProfileNames { get; set; }
    }
}
