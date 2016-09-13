using MySql.Data.MySqlClient;
using StowagePlanAnalytics_ITP_2016.Models;
using StowagePlanAnalytics_ITP_2016.Models.FileModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class DataGateway
    {
        private StowageDBContext db = new StowageDBContext();

        // VoyageFileCalculation database consistency SQL
        public static readonly string UpdateConsistencyWithVoyageFileCalculationTableSQL =
            // Table(s) with Foreign Key column that should only exist with relationship row 
            // in VoyageFileCalculation table:
            //      Voyage
            //      Files
            //      UsefulInfo
            //
            // Delete from Voyage table if relationship entry does not exist in VoyageFileCalculation table
            "DELETE FROM Voyage " +
            "WHERE VoyageID NOT IN ("
            + "SELECT VoyageID FROM VoyageFileCalculation);" +
            // Delete from Files table if relationship entry does not exist in VoyageFileCalculation table
            "DELETE FROM Files " +
            "WHERE Id NOT IN ("
            + "SELECT FileId FROM VoyageFileCalculation);" +
            // Delete from UsefulInfo table if relationship entry does not exist in VoyageFileCalculation tabledelete_sql +=
            "DELETE FROM UsefulInfo " +
            "WHERE TripID NOT IN ("
            + "SELECT UsefulInfoId FROM VoyageFileCalculation);"
            ;

        Dictionary<string, string> orderBydictionary = new Dictionary<string, string>();
        public DataGateway()
        {
            orderBydictionary.Add("CIPlanned", " DESC ");
            orderBydictionary.Add("Ballast", " ASC ");
            orderBydictionary.Add("TotalMoves", " ASC ");
        }
        public Service GetService(string serviceCode)
        {
            string query =
                "SELECT ServicePort.Id, Service.*, Port.*, ServicePort.SequenceNo, ServicePort.FileUpload " +
                "FROM ServicePort " +
                    "INNER JOIN Service ON ServicePort.ServiceCode = Service.ServiceCode " +
                    "INNER JOIN Port ON ServicePort.PortCode = Port.PortCode " +
                "WHERE Service.ServiceCode = @serviceCode " +
                "ORDER BY ServicePort.SequenceNo;";

            IEnumerable<ServicePort> result = db.Database.SqlQuery<ServicePort>(query, new MySqlParameter("serviceCode", serviceCode)).ToList();
            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            Service service = new Service();
            // Get service data from first result
            ServicePort firstResultRow = result.First();
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
        public List<SelectListItem> GetClassListItems(string VesselTEUClassCode)
        {
            List<SelectListItem> ClasslistItems = new List<SelectListItem>();
            List<string> classList = GetClassList();
            for (int i = 0; i < classList.Count(); i++)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = classList[i], Value = classList[i] };
                if (classList[i] == VesselTEUClassCode)
                {
                    selectListItem.Selected = true;
                }
                ClasslistItems.Add(selectListItem);
            }
            return ClasslistItems;
        }
        public List<SelectListItem> GetServiceListItems(string serviceCode)
        {
            List<SelectListItem> ServicelistItems = new List<SelectListItem>();
            List<string> serviceList = GetServiceList();
            for (int i = 0; i < serviceList.Count(); i++)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = serviceList[i], Value = serviceList[i] };
                if (serviceList[i] == serviceCode)
                {
                    selectListItem.Selected = true;
                }
                ServicelistItems.Add(selectListItem);
            }
            return ServicelistItems;
        }
        public List<string> GetServiceList()
        {
            string query = "SELECT ServiceCode FROM Service";
            IEnumerable<string> result = db.Database.SqlQuery<string>(query).ToList();
            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            List<string> serviceList = new List<string>();
            // Populate the serviceList with the query result
            serviceList.Add("All");
            foreach (var row in result)
            {
                serviceList.Add(row);
            }
            return serviceList;
        }
        public List<string> GetClassList()
        {
            string query = "SELECT VesselTEUClassCode FROM Class";
            IEnumerable<string> result = db.Database.SqlQuery<string>(query).ToList();
            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            List<string> serviceList = new List<string>();
            // Populate the serviceList with the query result
            foreach (var row in result)
            {
                serviceList.Add(row);
            }
            return serviceList;
        }
        public Port getPortDetails(string portCode)
        {
            //query to get details of portCode and return it as a port object
            string query = "SELECT * FROM Port WHERE PortCode = '" + portCode + "'";
            IEnumerable<Port> result = db.Database.SqlQuery<Port>(query).ToList();
            Port port = new Port();
            port.CostOfMove = result.First().CostOfMove;
            port.NoOfCranes = result.First().NoOfCranes;
            port.PortCode = result.First().PortCode;
            return port;

        }

        //Teck Loon: I changed to retrieve portname also for populating the create service side.
        public List<string> GetAllPortList()
        {
            string query = "SELECT PortCode FROM Port";
            IEnumerable<string> result = db.Database.SqlQuery<string>(query).ToList();

            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            List<string> AllPortList = new List<string>();
            // Populate the serviceList with the query result
            foreach (var row in result)
            {
                AllPortList.Add(row);
            }
            return AllPortList;
        }
        public Vessel GetVesselDetails(string vesselName)
        {
            // first query: get each bay's hatch pattern,maximum tier for the vesselName(i.e vessel's structure)
            string query = "SELECT Bay,HatchPattern, Max(Tier) as MaxTier FROM Structure s, Vessel v WHERE s.VesselTEUClassCode = v.VesselTEUClassCode AND v.VesselName = '" + vesselName + "' GROUP BY Bay;";
            IEnumerable<Structure> result = db.Database.SqlQuery<Structure>(query).ToList();
            SortedDictionary<int, Structure> bayDictionary = new SortedDictionary<int, Structure>();
            Vessel vessel = new Vessel();
            foreach (var row in result)
            {
                Structure bay = new Structure();
                bay.Bay = row.Bay;
                bay.HatchPattern = row.HatchPattern;
                bay.MaxTier = row.MaxTier;
                bayDictionary[bay.Bay] = bay;

            }
            vessel.bayDictionary = bayDictionary;
            //second query: get vesselName's details(i.e max capacity,max weight,max reefers)
            string query2 = "SELECT * FROM Vessel WHERE VesselName = '" + vesselName + "' ";
            IEnumerable<Vessel> result2 = db.Database.SqlQuery<Vessel>(query2).ToList();
            vessel.TEUCapacity = result2.First().TEUCapacity;
            vessel.MaxWeight = result2.First().MaxWeight;
            vessel.MaxReefers = result2.First().MaxReefers;
            vessel.VesselTEUClassCode = result2.First().VesselTEUClassCode;
            vessel.VesselName = result2.First().VesselName;
            vessel.VesselCode = result2.First().VesselCode;

            return vessel;
        }
        public List<string> GetPortList(string serviceCode)
        {
            string query = "SELECT DISTINCT PortCode FROM ServicePort WHERE ServiceCode = '" + serviceCode + "'";
            IEnumerable<string> result = db.Database.SqlQuery<string>(query).ToList();
            // If no result found,
            if (result.Count() == 0)
            {
                // return null model
                return null;
            }

            List<string> PortList = new List<string>();
            // Populate the serviceList with the query result
            foreach (var row in result)
            {
                PortList.Add(row);
            }
            return PortList;
        }
        public void CreateService(Service service)
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
        private string GetLastInsertedId(MySqlConnection conn, MySqlTransaction txn)
        {
            var resultReader = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, txn).ExecuteReader();
            resultReader.Read();
            var LAST_INSERT_ID = Convert.ToString(resultReader.GetValue(0));
            resultReader.Close();
            return LAST_INSERT_ID;
        }

        // Retrieve all Voyages
        public IEnumerable<Voyage> GetAllVoyages()
        {
            return db.Voyage.ToList();
        }

        // Retrieve Uploaded Files by VoyageID
        public IEnumerable<STIFFile> GetSTIFFilesByVoyageID(string voyageID)
        {
            string sql =
                "SELECT VoyageFileCalculation.FileId, Files.FileName, ServicePort.PortCode, ServicePort.SequenceNo, VoyageFileCalculation.UsefulInfoId " +
                "FROM VoyageFileCalculation "
                + "INNER JOIN Files ON VoyageFileCalculation.FileId = Files.Id "
                + "INNER JOIN ServicePort ON VoyageFileCalculation.ServicePortId = ServicePort.Id " +
                "WHERE VoyageFileCalculation.VoyageID = @voyageID;";
            IEnumerable<VoyageFileCalculation> result = db.Database.SqlQuery<VoyageFileCalculation>(sql, new MySqlParameter("voyageID", voyageID)).ToList();

            if (result.Count() > 0)
            {
                // Arrange result by SequenceNo
                result = result.OrderBy(r => r.SequenceNo);

                var stifFileList = new List<STIFFile>();
                foreach (var row in result)
                {
                    // Create UploadedFile object
                    var uploadedFile = new UploadedFile();
                    uploadedFile.Id = row.FileId;
                    uploadedFile.FileName = row.FileName;
                    // Create STIFFile object
                    var stifFile = new STIFFile();
                    stifFile.file = uploadedFile;
                    stifFile.portCode = row.PortCode;
                    stifFile.portSequence = row.SequenceNo;
                    stifFile.tripId = row.UsefulInfoId;
                    // Add STIFFile object to list
                    stifFileList.Add(stifFile);
                }
                return stifFileList;
            }
            else
            {
                return null;
            }

        }

        // Delete Voyage, Files and Calculations
        public bool DeleteUploadedFilesByVoyageID(string voyageID)
        {
            // Delete from VoyageFileCalculation table (relationship table)
            string delete_sql =
                "DELETE FROM VoyageFileCalculation " +
                "WHERE VoyageID = @voyageID;";
            var sqlAffectedRows =
                db.Database.ExecuteSqlCommand(
                    delete_sql + UpdateConsistencyWithVoyageFileCalculationTableSQL,
                    new MySqlParameter("voyageID", voyageID)
                );
            db.SaveChanges();
            if (sqlAffectedRows > 0)
                return true;
            else
                return false;
        }

        // Save files and calculations and build relationship table
        public void SaveFileAndCalculations(Voyage voyage, UploadedFile[] files, UsefulInfo[] usefulInfo)
        {
            //Testing variables
            var firstCount = files.Where(f => f.Id != 0).Count();
            var firstCheck = firstCount == 0;
            var secondCount = usefulInfo.Where(u => u.TripID != 0).Count();
            var secondCheck = secondCount == 0;

            // If both uploadedfiles Id and usefulinfo Id not set,
            if (files.Where(f => f.Id != 0).Count() == 0 && usefulInfo.Where(u => u.TripID != 0).Count() == 0)
            {
                InsertVoyageAndFilesAndCalculations(voyage, files, usefulInfo);
            }
            else
            {
                // Attempt to update database with new calculations
                ReplaceUploadedFilesAndUpdateCalculations(files, usefulInfo);
            }
        }
        // This class is used as a temporary model to store results from database query
        // Currently used in functions:
        //      GetSTIFFilesByVoyageID(string voyageID)
        internal class VoyageFileCalculation
        {
            // Files table fields
            public int FileId { get; set; }
            public string FileName { get; set; }
            // ServicePort table fields
            public string PortCode { get; set; }
            public int SequenceNo { get; set; }
            public int UsefulInfoId { get; set; }
        }


        public void InsertVoyageAndFilesAndCalculations(Voyage voyage, UploadedFile[] files, UsefulInfo[] usefulInfo)
        {
            // Check input parameters match up
            if (files.Length != usefulInfo.Length || files.Length != voyage.Trips.Count())
            {
                throw new Exception("Parameters do not match up. Calculation and Files not stored in database.");
            }

            // Manually handle transaction (referenced from https://msdn.microsoft.com/en-us/data/dn456843.aspx)
            using (MySqlConnection mySqlConn = (MySqlConnection)db.Database.Connection)
            {
                mySqlConn.Open();
                using (MySqlTransaction mySqlTxn = (MySqlTransaction)mySqlConn.BeginTransaction())
                {
                    try
                    {
                        // SQL Command for Insert into Voyage Table
                        var mysql_insert_voyage = new StringBuilder(
                            "INSERT INTO Voyage (VoyageID, ServiceCode, VesselCode) VALUES ('@VoyageID', '@ServiceCode', '@VesselCode')")
                            .Replace("@VoyageID", voyage.VoyageID)
                            .Replace("@ServiceCode", voyage.ServiceCode)
                            .Replace("@VesselCode", voyage.Vessel.VesselCode)
                            .ToString();
                        // Execute Voyage table row insertion and verify success
                        if (new MySqlCommand(mysql_insert_voyage, mySqlConn, mySqlTxn).ExecuteNonQuery() != 1)
                            throw new Exception("Failed to insert Voyage.");

                        // Repeat Insert File/UsefulInfo/Relationship table for number of files
                        for (int i = 0; i < files.Length; i++)
                        {
                            // SQL Command for Insert into Files Table
                            var mysql_insert_files = new StringBuilder(
                                "INSERT INTO Files (FileName, FileContent) VALUES ('@FileName', 0x@FileContent)")
                                .Replace("@FileName", files[i].FileName)
                                .Replace("@FileContent", files[i].FileBytesToHexString())
                                .ToString();
                            // Execute File table row insertion and verify success
                            if (new MySqlCommand(mysql_insert_files, mySqlConn, mySqlTxn).ExecuteNonQuery() != 1)
                                throw new Exception("Failed to insert File: " + files[i].FileName + ".");
                            // Retrieve Inserted File Id
                            var file_insert_id = GetLastInsertedId(mySqlConn, mySqlTxn);

                            // SQL Command for Insert into UsefulInfo
                            var mysql_insert_usefulinfo = new StringBuilder(
                                "INSERT INTO UsefulInfo ("
                                + "VoyageID, DepPortCode, ServiceCode, ArrPortCode, VesselCode, "
                                + "RemainingReefersSlots, ReefersDischarged, ReefersLoaded, ReefersOnboard, ReefersUtilisation, ReefersCapacity, "
                                + "TEURemaining, TEUOnboard, TEUUtilisation, TEUCapacity, "
                                + "IMOUnits, IMOUtilisation, OOGUnits, OOGUtilisation, "
                                + "MaxWeight, WeightOnboard, WeightRemaining, WeightUtilisation, "
                                + "Ballast, BunkerHFO, BunkerMGOMDO, "
                                + "LoadedMoves, DischargedMoves, RestowMoves, TotalMoves, RestowPercentage, RestowCost, "
                                + "CIAgreed, CIPlanned, OwnerCount, VesselTEUClassCode, CraneAllocation, ReshiftingMoves, "
                                + "FileId) " +
                                "VALUES ("
                                + "'@VoyageID', '@DepPortCode', '@ServiceCode', '@ArrPortCode', '@VesselCode', "
                                + "@RemainingReefersSlots, @ReefersDischarged, @ReefersLoaded, @ReefersOnboard, @ReefersUtilisation, @ReefersCapacity, "
                                + "@TEURemaining, @TEUOnboard, @TEUUtilisation, @TEUCapacity, "
                                + "@IMOUnits, @IMOUtilisation, @OOGUnits, @OOGUtilisation, "
                                + "@MaxWeight, @WeightOnboard, @WeightRemaining, @WeightUtilisation, "
                                + "@Ballast, @BunkerHFO, @BunkerMGOMDO, "
                                + "@LoadedMoves, @DischargedMoves, @RestowMoves, @TotalMoves, @RestowPercentage, @RestowCost, "
                                + "@CIAgreed, @CIPlanned, '@OwnerCount', '@VesselTEUClassCode', '@CraneAllocation',@ReshiftingMoves, "
                                + "@FileId)")
                                .Replace("@VoyageID", usefulInfo[i].VoyageID).Replace("@DepPortCode", usefulInfo[i].DepPortCode).Replace("@ServiceCode", usefulInfo[i].ServiceCode)
                                .Replace("@ArrPortCode", usefulInfo[i].ArrPortCode).Replace("@VesselCode", usefulInfo[i].VesselCode)
                                //Reefers
                                .Replace("@RemainingReefersSlots", usefulInfo[i].RemainingReefersSlots.ToString()).Replace("@ReefersDischarged", usefulInfo[i].ReefersDischarged.ToString())
                                .Replace("@ReefersLoaded", usefulInfo[i].ReefersLoaded.ToString()).Replace("@ReefersOnboard", usefulInfo[i].ReefersOnboard.ToString())
                                .Replace("@ReefersUtilisation", usefulInfo[i].ReefersUtilisation.ToString()).Replace("@ReefersCapacity", usefulInfo[i].ReefersCapacity.ToString())
                                //TEU
                                .Replace("@TEURemaining", usefulInfo[i].TEURemaining.ToString()).Replace("@TEUOnboard", usefulInfo[i].TEUOnboard.ToString())
                                .Replace("@TEUUtilisation", usefulInfo[i].TEUUtilisation.ToString()).Replace("@TEUCapacity", usefulInfo[i].TEUCapacity.ToString())
                                //IMO & OOG
                                .Replace("@IMOUnits", usefulInfo[i].IMOUnits.ToString()).Replace("@IMOUtilisation", usefulInfo[i].IMOUtilisation.ToString())
                                .Replace("@OOGUnits", usefulInfo[i].OOGUnits.ToString()).Replace("@OOGUtilisation", usefulInfo[i].OOGUtilisation.ToString())
                                //Weight
                                .Replace("@MaxWeight", usefulInfo[i].MaxWeight.ToString()).Replace("@WeightOnboard", usefulInfo[i].WeightOnboard.ToString())
                                .Replace("@WeightRemaining", usefulInfo[i].WeightRemaining.ToString()).Replace("@WeightUtilisation", usefulInfo[i].WeightUtilisation.ToString())
                                //Ballast & Bunker
                                .Replace("@Ballast", usefulInfo[i].Ballast.ToString()).Replace("@BunkerHFO", usefulInfo[i].BunkerHFO.ToString()).Replace("@BunkerMGOMDO", usefulInfo[i].BunkerMGOMDO.ToString())
                                //Moves
                                .Replace("@LoadedMoves", usefulInfo[i].LoadedMoves.ToString()).Replace("@DischargedMoves", usefulInfo[i].DischargedMoves.ToString())
                                .Replace("@RestowMoves", usefulInfo[i].RestowMoves.ToString()).Replace("@TotalMoves", usefulInfo[i].TotalMoves.ToString())
                                .Replace("@RestowPercentage", usefulInfo[i].RestowPercentage.ToString()).Replace("@RestowCost", usefulInfo[i].RestowCost.ToString())
                                //CI & Owner Count & Vessel Class
                                .Replace("@CIAgreed", usefulInfo[i].CIAgreed.ToString()).Replace("@CIPlanned", usefulInfo[i].CIPlanned.ToString()).Replace("@OwnerCount", usefulInfo[i].OwnerCount)
                                .Replace("@VesselTEUClassCode", usefulInfo[i].VesselTEUClassCode).Replace("@CraneAllocation", usefulInfo[i].CraneAllocation)
                                .Replace("@ReshiftingMoves", usefulInfo[i].ReshiftingMoves.ToString())
                                //File Id
                                .Replace("@FileId", file_insert_id)
                                .ToString();
                            // Execute UsefulInfo table row insertion and verify success
                            if (new MySqlCommand(mysql_insert_usefulinfo, mySqlConn, mySqlTxn).ExecuteNonQuery() != 1)
                                throw new Exception("Failed to insert Calculations.");
                            // Retrieve Inserted UsefulInfo Id
                            var usefulinfo_insert_id = GetLastInsertedId(mySqlConn, mySqlTxn);

                            // SQL Command for Relationship table
                            //TODO: SQL for insert relationship
                            var mysql_insert_relationship = new StringBuilder(
                                "INSERT INTO VoyageFileCalculation (VoyageID, FileId, ServicePortId, UsefulInfoId) " +
                                "VALUES ('@VoyageID', @FileId, @ServicePortId, @UsefulInfoId)")
                                .Replace("@VoyageID", voyage.VoyageID)
                                .Replace("@FileId", file_insert_id)
                                .Replace("@ServicePortId", voyage.Trips.ElementAt(i).DepPort.ServicePortId.ToString())
                                .Replace("@UsefulInfoId", usefulinfo_insert_id)
                                .ToString();
                            // Execute Relationship table row insertion and verify success
                            if (new MySqlCommand(mysql_insert_relationship, mySqlConn, mySqlTxn).ExecuteNonQuery() != 1)
                                throw new Exception("Failed to build data relationship.");
                        }

                        mySqlTxn.Commit();
                        mySqlConn.Close();
                    }
                    catch (Exception ex)
                    {
                        mySqlTxn.Rollback();
                        mySqlConn.Close();
                        throw new Exception(ex.Message + " Calculation and Files not stored in database.");
                    }
                }
            }
        }

        // Replace files in voyage
        public void ReplaceUploadedFilesAndUpdateCalculations(IEnumerable<UploadedFile> files, IEnumerable<UsefulInfo> usefulInfo)
        {
            if (files.Count() != 0)
            {
                string update_sql = "";
                List<MySqlParameter> mysqlParametersList = new List<MySqlParameter>();
                int x = 0;
                foreach (var file in files)
                {
                    update_sql +=
                        "UPDATE Files SET FileName=@FileName" + x + ", FileContent=@FileContent" + x + " " +
                        "WHERE Id=@Id" + x + ";";
                    mysqlParametersList.Add(new MySqlParameter("FileName" + x, file.FileName));
                    mysqlParametersList.Add(new MySqlParameter("FileContent" + x, file.FileContent));
                    mysqlParametersList.Add(new MySqlParameter("Id" + x, file.Id));
                    x++;
                }
                int y = 0;
                foreach (var info in usefulInfo)
                {
                    update_sql +=
                        "UPDATE UsefulInfo SET "
                        + "RemainingReefersSlots=@RemainingReefersSlots" + y + ", ReefersDischarged=@ReefersDischarged" + y + ", ReefersLoaded=@ReefersLoaded" + y + ", ReefersOnboard=@ReefersOnboard" + y + ", ReefersUtilisation=@ReefersUtilisation" + y + ", ReefersCapacity=@ReefersCapacity" + y + ", "
                        + "TEURemaining=@TEURemaining" + y + ", TEUOnboard=@TEUOnboard" + y + ", TEUUtilisation=@TEUUtilisation" + y + ", TEUCapacity=@TEUCapacity" + y + ", "
                        + "IMOUnits=@IMOUnits" + y + ", IMOUtilisation=@IMOUtilisation" + y + ", OOGUnits=@OOGUnits" + y + ", OOGUtilisation=@OOGUtilisation" + y + ", "
                        + "MaxWeight=@MaxWeight" + y + ", WeightOnboard=@WeightOnboard" + y + ", WeightRemaining=@WeightRemaining" + y + ", WeightUtilisation=@WeightUtilisation" + y + ", "
                        + "Ballast=@Ballast" + y + ", BunkerHFO=@BunkerHFO" + y + ", BunkerMGOMDO=@BunkerMGOMDO" + y + ", "
                        + "LoadedMoves=@LoadedMoves" + y + ", DischargedMoves=@DischargedMoves" + y + ", RestowMoves=@RestowMoves" + y + ", TotalMoves=@TotalMoves" + y + ", RestowPercentage=@RestowPercentage" + y + ", RestowCost=@RestowCost" + y + ", "
                        + "CIAgreed=@CIAgreed" + y + ", CIPlanned=@CIPlanned" + y + ", OwnerCount=@OwnerCount" + y + " " +
                        "WHERE TripID=@TripID" + y + ";";
                    mysqlParametersList.Add(new MySqlParameter("RemainingReefersSlots" + y, info.RemainingReefersSlots));
                    mysqlParametersList.Add(new MySqlParameter("ReefersDischarged" + y, info.ReefersDischarged));
                    mysqlParametersList.Add(new MySqlParameter("ReefersLoaded" + y, info.ReefersLoaded));
                    mysqlParametersList.Add(new MySqlParameter("ReefersOnboard" + y, info.ReefersOnboard));
                    mysqlParametersList.Add(new MySqlParameter("ReefersUtilisation" + y, info.ReefersUtilisation));
                    mysqlParametersList.Add(new MySqlParameter("ReefersCapacity" + y, info.ReefersCapacity));
                    mysqlParametersList.Add(new MySqlParameter("TEURemaining" + y, info.TEURemaining));
                    mysqlParametersList.Add(new MySqlParameter("TEUOnboard" + y, info.TEUOnboard));
                    mysqlParametersList.Add(new MySqlParameter("TEUUtilisation" + y, info.TEUUtilisation));
                    mysqlParametersList.Add(new MySqlParameter("TEUCapacity" + y, info.TEUCapacity));
                    mysqlParametersList.Add(new MySqlParameter("IMOUnits" + y, info.IMOUnits));
                    mysqlParametersList.Add(new MySqlParameter("IMOUtilisation" + y, info.IMOUtilisation));
                    mysqlParametersList.Add(new MySqlParameter("OOGUnits" + y, info.OOGUnits));
                    mysqlParametersList.Add(new MySqlParameter("OOGUtilisation" + y, info.OOGUtilisation));
                    mysqlParametersList.Add(new MySqlParameter("MaxWeight" + y, info.MaxWeight));
                    mysqlParametersList.Add(new MySqlParameter("WeightOnboard" + y, info.WeightOnboard));
                    mysqlParametersList.Add(new MySqlParameter("WeightRemaining" + y, info.WeightRemaining));
                    mysqlParametersList.Add(new MySqlParameter("WeightUtilisation" + y, info.WeightUtilisation));
                    mysqlParametersList.Add(new MySqlParameter("Ballast" + y, info.Ballast));
                    mysqlParametersList.Add(new MySqlParameter("BunkerHFO" + y, info.BunkerHFO));
                    mysqlParametersList.Add(new MySqlParameter("BunkerMGOMDO" + y, info.BunkerMGOMDO));
                    mysqlParametersList.Add(new MySqlParameter("LoadedMoves" + y, info.LoadedMoves));
                    mysqlParametersList.Add(new MySqlParameter("DischargedMoves" + y, info.DischargedMoves));
                    mysqlParametersList.Add(new MySqlParameter("RestowMoves" + y, info.RestowMoves));
                    mysqlParametersList.Add(new MySqlParameter("TotalMoves" + y, info.TotalMoves));
                    mysqlParametersList.Add(new MySqlParameter("RestowPercentage" + y, info.RestowPercentage));
                    mysqlParametersList.Add(new MySqlParameter("RestowCost" + y, info.RestowCost));
                    mysqlParametersList.Add(new MySqlParameter("CIAgreed" + y, info.CIAgreed));
                    mysqlParametersList.Add(new MySqlParameter("CIPlanned" + y, info.CIPlanned));
                    mysqlParametersList.Add(new MySqlParameter("OwnerCount" + y, info.OwnerCount));
                    mysqlParametersList.Add(new MySqlParameter("TripID" + y, info.TripID));
                    y++;
                }
                db.Database.ExecuteSqlCommand(update_sql, mysqlParametersList.ToArray());
                db.SaveChanges();
            }
        }

        public UploadedFile GetUploadedFileById(int fileId)
        {
            string sql =
                "SELECT Files.* " +
                "FROM Files " +
                "WHERE Files.Id = @fileId";
            return db.Database.SqlQuery<UploadedFile>(sql, new MySqlParameter("fileId", fileId)).ToList().First();
        }

        internal class ServicePort
        {
            public int Id { get; set; }
            public string ServiceCode { get; set; }
            public string PortCode { get; set; }
            public string PortName { get; set; }
            public int NoOfCranes { get; set; }
            public double CostOfMove { get; set; }
            public int SequenceNo { get; set; }
            public bool FileUpload { get; set; }
        }

        // Function to convert byte array contents to hexadecimal string
        //ref: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        //SEARCH

        // does intital search with no piroirty sorting

        public IEnumerable<UsefulInfo> search(FormCollection collection)
        {
            //starting query string
            String query = "SELECT * FROM UsefulInfo";
            //list which holds all the where conditions(e.g WHERE CIAgreeded = 0)
            List<string> whereConditions = new List<string>();
            //variable to keep the count of the formcollection
            int count = 0;
            foreach (var key in collection.AllKeys)
            {
                count++;

                //formcollection is a dictonary so each key is actually a column name in
                //usefulinfo table(i.e the first key would be actually serviceCode)
                var columnName = key;
                //for each key's value,it will be the the value searching(e.g serviceCode==sv1)
                var value = collection[key];
                if (value != null && value != "All" && value != "")
                {
                    //adding the string of where conditions with = and a space
                    whereConditions.Add(columnName + " = '" + value + "'");
                }
                //the form element at 5 is a button
                if (count == 5)
                {
                    break;
                }
            }


            string whereJoiner = " WHERE ";
            //join the strings to make a SQL statement
            foreach (string wherecondition in whereConditions)
            {
                query += whereJoiner + wherecondition;
                whereJoiner = " AND ";
            }

            return db.Database.SqlQuery<UsefulInfo>(query).ToList();






        }

        // does advanced search with  piroirty sorting and searching
        public IEnumerable<UsefulInfo> advanceSearch(FormCollection collection)
        {
            //starting query string
            String query = "SELECT * FROM UsefulInfo";
            //list which holds all the where conditions(e.g WHERE CIAgreeded = 0)
            List<string> whereConditions = new List<string>();
            //list which holds all the order by conditions(e.g order by CIAgreeded ASC)
            List<string> orderByConditions = new List<string>();
            //variable to keep the count of the formcollection
            int count = 0;
            foreach (var key in collection.AllKeys)
            {
                count++;
                //formcollection is a dictonary so each key is actually a column name in
                //usefulinfo table(i.e the first key would be actually serviceCode)
                var columnName = key;
                //for each key's value,it will be the the value searching(e.g serviceCode==sv1)
                var value = collection[key];
                if (value != null && value != "All" && value != "")
                {
                    //adding the string of where conditions with = and a space
                    whereConditions.Add(columnName + " = '" + value + "'");
                }
                if (count == 5)
                {//the form element at 5 is a button
                    break;
                }
            }
            for (int countSecond = 5; countSecond < collection.Count - 1; countSecond += 3)
            {
                //after the button carry on the adding again
                var property = collection.Get(countSecond);
                //add the orderby(e.g piroirty 1 is ballast
                orderByConditions.Add(property);
                //get the minium value of the property
                var minValue = collection.Get(countSecond + 1);
                //get the max value of the property
                var maxValue = collection.Get(countSecond + 2);
                if (minValue != null && maxValue != null && (double.Parse(minValue) < double.Parse(maxValue)))
                {
                    //add the where condition string(e.g CIPlanned between 10 AND 20)
                    whereConditions.Add(property + " BETWEEN " + minValue + " AND " + maxValue);
                }


            }
            string whereJoiner = " WHERE ";
            //join the strings to make a SQL statement
            foreach (string wherecondition in whereConditions)
            {
                query += whereJoiner + wherecondition;
                whereJoiner = " AND ";
            }
            string orderByJoiner = " ORDER BY ";
            //join the strings to make a SQL statement
            foreach (string orderByCondition in orderByConditions.AsEnumerable().Reverse())
            {
                //check for each orderbyCondition how should it be ordered by(e.g by checking
                //orderByDictonary can see that CIplanned should be ordered DESC
                query += orderByJoiner + orderByCondition + orderBydictionary[orderByCondition];
                orderByJoiner = " , ";
            }

            return db.Database.SqlQuery<UsefulInfo>(query).ToList();
        }

        //does valdiation on the search criteria of advance search/sort plan(i.e minval cannot be more than max)
        public bool validateAdvanceSearchCriteria(FormCollection collection)
        {
            for (int count = 5; count < collection.Count - 1; count += 3)
            {
                //after the button carry on the adding again
                var property = collection.Get(count);

                //get the minium value of the property
                var minValue = collection.Get(count + 1);
                //get the max value of the property
                var maxValue = collection.Get(count + 2);
                if ( (minValue == "0" && maxValue == "0") || minValue == null || maxValue == null || (double.Parse(minValue) > double.Parse(maxValue)) || (double.Parse(minValue) < 0) || (double.Parse(maxValue) < 0))
                {
                    return false;
                }


            }
            return true;
        }

    }
}