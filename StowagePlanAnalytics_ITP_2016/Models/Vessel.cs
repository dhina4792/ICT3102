using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Vessel
    {
        [Key]
        [Required]
        [StringLength(3, ErrorMessage = "Vessel Code can only contain a maximum of 3 characters.")]
        [Display(Name = "Vessel Code")]
        public string VesselCode { get; set; }

        [Display(Name = "Vessel Name")]
        [Required]
        [StringLength(100, ErrorMessage = "Vessel Name can only contain a maximum of 100 characters.")]
        public string VesselName { get; set; }

        [Display(Name = "Vessel TEU Class Code")]
        [Required]
        public string VesselTEUClassCode { get; set; }

        [Display(Name = "TEU Capacity")]
        [Required]
        public int TEUCapacity { get; set; }

        [Display(Name = "Max Weight")]
        [Required]
        public int MaxWeight { get; set; }

        [Display(Name = "Max Reefers")]
        [Required]
        public int MaxReefers { get; set; }

        [NotMapped]
        public SortedDictionary<int, Structure> bayDictionary { get; set; }
    }
}