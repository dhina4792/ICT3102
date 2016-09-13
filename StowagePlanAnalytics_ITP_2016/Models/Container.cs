using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Container
    {
        public string containerID;
        public string equipmentISO;
        public int weight;
        public string owner;
        public int stowLocation;
        public int bay;
        public int row;
        public int tier;
        public bool reefer;
        public string loadPort;
        public string dischargePort;
        public bool imo;
        public bool oog;

    }
}