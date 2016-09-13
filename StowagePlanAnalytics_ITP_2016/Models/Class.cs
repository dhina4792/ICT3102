using System.ComponentModel.DataAnnotations;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Class
    {
        [Key]
        [Display(Name = "Vessel TEU Class Code")]
        public string VesselTEUClassCode { get; set; }

        public string VesselTEUClass { get; set; }

    }
}