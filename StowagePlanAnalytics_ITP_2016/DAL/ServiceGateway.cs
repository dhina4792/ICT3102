using MySql.Data.MySqlClient;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class ServiceGateway : CRUDGateway<Service>
    {
        public override Service SelectByPrimaryKey(string serviceCode)
        {
            string query =
                "SELECT ServicePort.Id, Service.*, Port.*, ServicePort.SequenceNo, ServicePort.FileUpload " +
                "FROM ServicePort " +
                    "INNER JOIN Service ON ServicePort.ServiceCode = Service.ServiceCode " +
                    "INNER JOIN Port ON ServicePort.PortCode = Port.PortCode " +
                "WHERE Service.ServiceCode = @serviceCode " +
                "ORDER BY ServicePort.SequenceNo;";

            IEnumerable<DataGateway.ServicePort> result = db.Database.SqlQuery<DataGateway.ServicePort>(query, new MySqlParameter("serviceCode", serviceCode)).ToList();
            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            Service service = new Service();
            // Get service data from first result
            var firstResultRow = result.First();
            // Populate Service data fields
            service.ServiceCode = firstResultRow.ServiceCode;
            service.Ports = new List<Port>();
            foreach (var row in result)
            {
                Port port = new Port();
                port.PortCode = row.PortCode;
                port.PortName = row.PortName;
                port.NoOfCranes = row.NoOfCranes;
                port.CostOfMove = row.CostOfMove;
                port.ServicePortId = row.Id;
                port.SequenceNo = row.SequenceNo;
                port.FileUpload = row.FileUpload;
                service.Ports.Add(port);
            }

            return service;
        }

        public override void Insert(Service service)
        {
            // SQL to create service entry in Service table
            string sql = "INSERT INTO Service (ServiceCode) VALUES (@serviceCode);";
            int x = 0;
            List<MySqlParameter> mysqlParametersList = new List<MySqlParameter>();
            mysqlParametersList.Add(new MySqlParameter("serviceCode", service.ServiceCode));
            foreach (var serviceport in service.Ports)
            {
                sql += "INSERT INTO ServicePort (ServiceCode, PortCode, SequenceNo, FileUpload) " +
                    "VALUES (@serviceCode, @p" + x + ", @p" + (x + 1) + ", @p" + (x + 2) + ");";
                mysqlParametersList.Add(new MySqlParameter("p" + x, serviceport.PortCode));
                mysqlParametersList.Add(new MySqlParameter("p" + (x + 1), serviceport.SequenceNo));
                mysqlParametersList.Add(new MySqlParameter("p" + (x + 2), serviceport.FileUpload));
                x += 3; // Increase parameter index by 3
            }
            db.Database.ExecuteSqlCommand(sql, mysqlParametersList.ToArray());
            db.SaveChanges();
        }

        public override Service Delete(string key)
        {
            string delete_sql =
                "DELETE FROM Service " +
                "WHERE ServiceCode = @serviceCode;";
            // Add consistency sql to delete sql
            delete_sql += DataGateway.UpdateConsistencyWithVoyageFileCalculationTableSQL;
            // Execute deletion sql statement (deletion from service table cascades to ServicePort and Voyage tables)
            var result = db.Database.ExecuteSqlCommand(delete_sql, new MySqlParameter("serviceCode", key));
            // If rows are deleted in the database,
            if (result > 0)
            {
                // Create service object to signify delete success (to be consistent with other existing CRUD behavior)
                var service = new Service();
                service.ServiceCode = key;
                return service;
            }
            else
            {
                return null;
            }
        }
    }
}