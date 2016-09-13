using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Service
    {
        [Key]
        [Required(ErrorMessage = "Service Code is required")]
        [StringLength(50, ErrorMessage = "Service Code must be less than 50 characters long")]
        [Display(Name = "Service Code")]
        public string ServiceCode { get; set; }

        //[NotMapped]
        //public string PortCode { get; set; }
        //[NotMapped]
        //public string PortName { get; set; }
        //[NotMapped]
        //public string FileUpload { get; set; }

        // Disable Mapping by EntityFramework
        [NotMapped]
        //[Required(ErrorMessage = "All Port Code is required")]
        public virtual ICollection<Port> Ports { get; set; }

        [NotMapped]
        [Display(Name = "Service Name")]
        public string Name { get; set; }

        // This property will hold all available service for selection at upload page.
        [NotMapped]
        public IEnumerable<SelectListItem> Services { get; set; }
    }
}