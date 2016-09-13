using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Port
    {
        [Key]
        [Required(ErrorMessage = "Port Code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Port Code must have exactly 3 characters")]
        [Display(Name = "Port Code")]
        public string PortCode { get; set; }

        [Required(ErrorMessage = "Port Name is required")]
        [StringLength(100, ErrorMessage = "Port Name must be less than 100 characters long")]
        [Display(Name = "Port Name")]
        public string PortName { get; set; }

        [Required(ErrorMessage = "No Of Cranes is required")]
        [Display(Name = "No Of Cranes")]
        public int NoOfCranes { get; set; }

        [Required(ErrorMessage = "Cost Of Move is required")]
        [Display(Name = "Cost of Move (USD)")]
        public double CostOfMove { get; set; }

        // -- ServicePort columns
        [NotMapped]
        public int ServicePortId { get; set; }

        [NotMapped]
        public int SequenceNo { get; set; }

        [NotMapped]
        public bool FileUpload { get; set; }
        // --

        // Remove below attributes if not needed
        [NotMapped]
        public int id { get; set; }

        [NotMapped]
        [Display(Name = "Crane Intensity")]
        public double CraneIntensity { get; set; }

        [NotMapped]
        public int Crane { get; set; }
        
        [NotMapped]
        [Display(Name = "Move Duration")]
        public double MoveDuration { get; set; }
    }
}