using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models.ViewModels
{
    public class VM_STIFFileManagement_Index
    {
        [Display(Name = "Service Code")]
        public string ServiceCode { get; set; }
        public IEnumerable<Voyage> Voyages { get; set; }

        public class Voyage
        {
            public string VoyageID { get; set; }
        }
    }
}