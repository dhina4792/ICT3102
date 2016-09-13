using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class Trip
    {
        public int id;
        public string DeparturePort;
        public string ArrivalPort;
        public Port DepPort;
        public IEnumerable<Container> Containers;
        public int Ballast;
        public int BunkerOilHFO;
        public int BunkerOilMGOMDO;
    }
}