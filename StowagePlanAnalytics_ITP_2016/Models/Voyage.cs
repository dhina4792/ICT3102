using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Voyage
    {
        public string VoyageID { get; set; }
        public string ServiceCode { get; set; }
        public string VesselCode { get; set; }

        [NotMapped]
        public string VesselName;

        [NotMapped]
        public Vessel Vessel;

        [NotMapped]
        public IEnumerable<Trip> Trips;
    }
}