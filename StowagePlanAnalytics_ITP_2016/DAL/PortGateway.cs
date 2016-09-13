using MySql.Data.MySqlClient;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class PortGateway : CRUDGateway<Port>
    {
        private static readonly string PORT_TABLE = "Port";

        public PortGateway()
            : base()
        {
        }

        public void Update(Port port, string oldKey)
        {
            string sql =
                "UPDATE " + PORT_TABLE + " " +
                "SET PortCode=@PortCode, PortName=@PortName, NoOfCranes=@NoOfCranes, CostOfMove=@CostOfMove " +
                "WHERE PortCode=@oldKey";
            db.Database.ExecuteSqlCommand(sql,
                new MySqlParameter("PortCode", port.PortCode),
                new MySqlParameter("PortName", port.PortName),
                new MySqlParameter("NoOfCranes", port.NoOfCranes),
                new MySqlParameter("CostOfMove", port.CostOfMove),
                new MySqlParameter("oldKey", oldKey));
            db.SaveChanges();
        }
    }
}