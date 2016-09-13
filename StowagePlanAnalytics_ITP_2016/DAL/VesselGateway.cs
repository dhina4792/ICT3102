using MySql.Data.MySqlClient;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class VesselGateway : CRUDGateway<Vessel>
    {
        private static readonly string VESSEL_TABLE = "Vessel";

        public void Update(Vessel vessel, string oldKey)
        {
            string sql =
                "UPDATE " + VESSEL_TABLE + " " +
                "SET VesselCode=@VesselCode, VesselName=@VesselName, VesselTEUClassCode=@VesselTEUClassCode, TEUCapacity=@TEUCapacity, MaxWeight=@MaxWeight, MaxReefers=@MaxReefers " +
                "WHERE VesselCode=@oldKey";
            db.Database.ExecuteSqlCommand(sql,
                new MySqlParameter("VesselCode", vessel.VesselCode),
                new MySqlParameter("VesselName", vessel.VesselName),
                new MySqlParameter("VesselTEUClassCode", vessel.VesselTEUClassCode),
                new MySqlParameter("TEUCapacity", vessel.TEUCapacity),
                new MySqlParameter("MaxWeight", vessel.MaxWeight),
                new MySqlParameter("MaxReefers", vessel.MaxReefers),
                new MySqlParameter("oldKey", oldKey));
            db.SaveChanges();
        }

        public override Vessel Delete(string key)
        {
            var vessel = data.Find(key);

            if (vessel == null)
            {
                return null;
            }

            string delete_sql =
                "DELETE FROM " + VESSEL_TABLE + " " +
                "WHERE VesselCode = @VesselCode;";
            // Add consistency sql to delete sql
            delete_sql += DataGateway.UpdateConsistencyWithVoyageFileCalculationTableSQL;
            // Execute deletion sql statement (deletion from service table cascades to ServicePort and Voyage tables)
            db.Database.ExecuteSqlCommand(delete_sql, new MySqlParameter("VesselCode", key));
            return vessel;
        }
    }
}