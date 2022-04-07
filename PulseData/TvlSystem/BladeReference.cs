using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseData.TvlSystem
{
    [Table("BladeReference")]
    public class BladeReference
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("T_OFFSET")]
        public double TOffset { get; set; }

        [Column("T_SPACING")]
        public double TSpacing { get; set; }

        [Column("BLADE_WFM")]
        public byte[] BladeWaveform { get; set; }

        [Column("MIRROR_WFM")]
        public byte[] MirrorWaveform { get; set; }

        [Column("PROFILE_CURVATURE")]
        public double ProfileCurvature { get; set; }

        [Column("PROFILE_HEIGHT")]
        public double ProfileHeight { get; set; }

        [Column("PROFILE_CENTRE")]
        public double ProfileCentre { get; set; }

        public virtual ICollection<BladeProfileName> BladeProfileNames { get; set; }
    }
}
