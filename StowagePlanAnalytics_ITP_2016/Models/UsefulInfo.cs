using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StowagePlanAnalytics_ITP_2016.Models
{
    public class UsefulInfo
    {
        public string VoyageID { get; set; }

        [Key]
        public int TripID { get; set; }
        public string DepPortCode { get; set; }
        public string ServiceCode { get; set; }
        public string ArrPortCode { get; set; }
        public string VesselCode { get; set; }

        public int RemainingReefersSlots { get; set; }
        public int ReefersDischarged { get; set; }
        public int ReefersOnboard { get; set; }
        public int ReefersLoaded { get; set; }
        public int ReefersCapacity { get; set; }
        public double ReefersUtilisation { get; set; }

        public int TEURemaining { get; set; }
        public int TEUOnboard { get; set; }
        public int TEUCapacity { get; set; }
        public double TEUUtilisation { get; set; }

        public int IMOUnits { get; set; }
        public double IMOUtilisation { get; set; }

        public int OOGUnits { get; set; }
        public double OOGUtilisation { get; set; }

        public int MaxWeight { get; set; }
        public int WeightOnboard { get; set; }
        public int WeightRemaining { get; set; }
        public double WeightUtilisation { get; set; }

        public int Ballast { get; set; }
        public int BunkerHFO { get; set; }
        public int BunkerMGOMDO { get; set; }

        public int LoadedMoves { get; set; }
        public int DischargedMoves { get; set; }
        public int RestowMoves { get; set; }
        public int ReshiftingMoves { get; set; }        
        public int TotalMoves { get; set; }
        public double RestowPercentage { get; set; }
        public double RestowCost { get; set; }
        public double CIAgreed { get; set; }
        public double CIPlanned { get; set; }
        public string CraneAllocation { get; set; }
        public string OwnerCount { get; set; }
        public string VesselTEUClassCode { get; set; }

        public int FileId { get; set; }
    }
}