using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Structure
    {
        public int Bay { get; set; }
        public string HatchPattern { get; set; }
        public int MaxTier { get; set; }
    }
}