using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models.FileModel
{
    // Abstract class for file translation strategies
    public abstract class FileTranslationBase
    {
        public abstract string[] fileExtensionType();     // return file extension type in uppercase (eg. ".TXT")
        public abstract object Translate(params UploadedFile[] fileContent);
    }

    // File Translation Strategies (Translation strategies must inherit from abstract class FileTranslationBase)
    //STIF translation
    public class STIF : FileTranslationBase
    {
        // FILE CONSTANTS
        // file line 2 data columns
        private const int
            INDEX_VESSELNAME = 1,
            INDEX_VOYAGE = 2,
            INDEX_DEPARTPORT = 4,
            INDEX_NUMBEROFCONTAINERS = 6;
        //header names
        private const string
            HEADER_TANKS = "* TANKS";
        //container columns
        private const int
            VANS_EQUIPNUMBER = 0,
            VANS_EQUIPTYPEISO = 1,
            VANS_KGWEIGHT = 3,
            VANS_OWNER = 4,
            VANS_STOWLOCATION = 6,
            VANS_LIVEREEFER = 8,
            VANS_LOADPORT = 11,
            VANS_DISCHPORT = 12,
            VANS_IMOCODE = 16,
            VANS_OVRHEIGHT = 18,
            VANS_OVRWIDERIGHT = 19,
            VANS_OVRWIDELEFT = 20,
            VANS_OVRLONGFRONT = 21,
            VANS_OVRLONGBACK = 22;
        //tank columns
        private const int
            TANKS_TANKNAME = 0,
            TANKS_TANKTYPE = 1,
            TANKS_DEPARTTANK = 3;
        //comparison constants
        private const string
            ZERO = "0",
            Y = "Y";

        public override string[] fileExtensionType()
        {
            // Declare extension type matches in Uppercase
            return new string[] { ".STIF", ".TXT" };
        }

        public override object Translate(params UploadedFile[] files)
        {
            int currentFileIndex = 0;
            List<string> fileContentsList = new List<string>();
            // Convert file bytes to file content string (UTF-8 Encoding)
            foreach (var file in files)
            {
                fileContentsList.Add(file.FileBytesToString(System.Text.Encoding.UTF8));
            }
            string[] filesContents = fileContentsList.ToArray();

            // If vessel name and voyage not consistent for all files,
            if (!SameVesselNameAndVoyageID(filesContents))
                throw new Exception("Vessel Name or Voyage inconsistent");

            // Get Vessel Name and Voyage
            string[] line2Contents = GetContentsFromLine(2, filesContents[0]).Split('\t');
            string vesselName = line2Contents[INDEX_VESSELNAME];
            string voyageID = line2Contents[INDEX_VOYAGE];

            // Create Voyage model
            Voyage voyage = new Voyage();
            // Populate Voyage model
            voyage.VoyageID = voyageID;
            voyage.VesselName = vesselName;
            // Create List of Trip objects
            List<Trip> voyageTrips = new List<Trip>();
            voyage.Trips = voyageTrips;

            // For each file,
            try
            {
                for (int fileIterator = 0; fileIterator < filesContents.Length; fileIterator++)
                {
                    // Split file contents into string array, delimited by "\r\n", removing empty entries
                    string[] fileLines = filesContents[fileIterator].Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int numberOfLines = fileLines.Length;
                    int numberOfContainers = Int32.Parse(GetContentFromColumn(INDEX_NUMBEROFCONTAINERS, fileLines[1]));

                    // Create Trip object (1 file = 1 Trip)
                    Trip trip = new Trip();
                    // Populate trip model
                    trip.DeparturePort = GetContentFromColumn(INDEX_DEPARTPORT, fileLines[1]);
                    // Create List of Container objects
                    List<Container> tripContainers = new List<Container>();
                    trip.Containers = tripContainers;
                    trip.Ballast = 0;
                    trip.BunkerOilHFO = 0;
                    trip.BunkerOilMGOMDO = 0;

                    // For each row of container data, (First row of container data begins from FileRow 6, fileLines[5])
                    for (int fileLineNumber = 5; fileLineNumber < numberOfContainers + 5; fileLineNumber++)
                    {
                        string[] fileRowContent = fileLines[fileLineNumber].Split('\t');
                        // Create container object
                        Container container = new Container();
                        // Place data into container object
                        container.containerID = fileRowContent[VANS_EQUIPNUMBER];
                        container.equipmentISO = fileRowContent[VANS_EQUIPTYPEISO];
                        container.weight = Int32.Parse(fileRowContent[VANS_KGWEIGHT]);
                        container.owner = fileRowContent[VANS_OWNER];
                        container.stowLocation = Int32.Parse(fileRowContent[VANS_STOWLOCATION].Replace(" ", String.Empty));
                        // Break stow location string into bay/row/tier
                        string[] stowLocation = ConvertToBayRowTier(fileRowContent[VANS_STOWLOCATION]);
                        container.bay = Int32.Parse(stowLocation[0]);
                        container.row = Int32.Parse(stowLocation[1]);
                        container.tier = Int32.Parse(stowLocation[2]);
                        container.reefer = (fileRowContent[VANS_LIVEREEFER].Equals(Y)) ? true : false;
                        container.loadPort = fileRowContent[VANS_LOADPORT];
                        container.dischargePort = fileRowContent[VANS_DISCHPORT];
                        container.imo = (fileRowContent[VANS_IMOCODE].Equals(String.Empty)) ? false : true;
                        container.oog =
                            (fileRowContent[VANS_OVRHEIGHT].Equals(ZERO) &&
                            fileRowContent[VANS_OVRWIDERIGHT].Equals(ZERO) &&
                            fileRowContent[VANS_OVRWIDELEFT].Equals(ZERO) &&
                            fileRowContent[VANS_OVRLONGFRONT].Equals(ZERO) &&
                            fileRowContent[VANS_OVRLONGBACK].Equals(ZERO)) ? false : true;

                        // Add Container object to Trip containers list
                        System.Diagnostics.Debug.WriteLine("Container ID: " + container.containerID);

                        tripContainers.Add(container);
                    }

                    // Find file line index matching "* TANKS"
                    int rowIndexHeader_Tanks = GetHeaderIndex(fileLines, HEADER_TANKS, numberOfContainers + 5);
                    // If header not found,
                    if (rowIndexHeader_Tanks == -1)
                        throw new FileCorruptException("File header \"* TANKS\" not found." + ";" + currentFileIndex);

                    // Process data for * TANKS
                    for (int fileLineNumber = rowIndexHeader_Tanks + 1; fileLineNumber < fileLines.Length - 1; fileLineNumber++)
                    {
                        string[] fileRowContent = fileLines[fileLineNumber].Split('\t');
                        // Check if tank type is Ballast or Fuel
                        if (fileRowContent[TANKS_TANKTYPE].Equals("S"))
                        {
                            // Water Ballast Tank
                            trip.Ballast += Int32.Parse(fileRowContent[TANKS_DEPARTTANK]);
                        }
                        else if (fileRowContent[TANKS_TANKTYPE].Equals("F"))
                        {
                            // Fuel Tank
                            string tankName = fileRowContent[TANKS_TANKNAME];
                            // Check if tank is HFO or MDO/MGO
                            if (tankName.Contains("HFO") || tankName.Contains("H.F.O"))
                            {
                                // Add value to Bunker HFO
                                trip.BunkerOilHFO += Int32.Parse(fileRowContent[TANKS_DEPARTTANK]);
                            }
                            else if (tankName.Contains("MDO") || tankName.Contains("MGO") || tankName.Contains("M.D.O") || tankName.Contains("M.G.O"))
                            {
                                // Add value to Bunker MDO/MGO
                                trip.BunkerOilMGOMDO += Int32.Parse(fileRowContent[TANKS_DEPARTTANK]);
                            }
                        }
                    }
                    // Add Trip to Voyage
                    voyageTrips.Add(trip);
                    currentFileIndex++;
                }
            }
            catch (IndexOutOfRangeException nullEx)
            {
                throw new FileCorruptException(nullEx.Message + ";" + currentFileIndex);
            }
            // return Voyage object
            return voyage;
        }

        private int GetHeaderIndex(string[] fileLine, string searchString, int searchStartIndex = 0, int searchEndIndex = 0)
        {
            // If End Index not specified or 0,
            if (searchEndIndex == 0)
                searchEndIndex = fileLine.Length - 1;   // Set End Index to last index in fileLine

            for (int index = searchStartIndex; index <= searchEndIndex; index++)
            {
                // Get current row string
                string currentRowString = fileLine[index];
                // If current row string content matches search string content,
                if (currentRowString.Replace("\t", String.Empty).Equals(searchString))
                    return index;   // return fileLine index
            }
            // Search String not found; return -1
            return -1;
        }

        private string GetContentsFromLine(int lineNumber, string fileContents)
        {
            // If line number is 0 or less,
            if (lineNumber <= 0)
                return null;    // return null string

            // Split file contents into lines
            string[] fileLines = fileContents.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // If requested line number is not out-of-range of file,
            if (lineNumber <= fileLines.Length)
            {
                // return file line
                return fileLines[lineNumber - 1];
            }
            else
            {
                // Otherwise, return null string
                return null;
            }

        }

        private string GetContentFromColumn(int columnIndex, string fileLineContents)
        {
            string[] lineContents = fileLineContents.Split('\t');
            return lineContents[columnIndex];
        }

        private bool SameVesselNameAndVoyageID(params string[] filesContents)
        {
            // Set iterator to last file index
            int filesContentsIterator = filesContents.Length - 1;

            while (filesContentsIterator > 0)
            {
                // Get contents from 2nd line of files contents
                string fileLine2ContentsIteratorFile = GetContentsFromLine(2, filesContents[filesContentsIterator]);
                string fileLine2ContentsNextFile = GetContentsFromLine(2, filesContents[filesContentsIterator - 1]);
                // Split
                string[] fileLine2Iter = fileLine2ContentsIteratorFile.Split('\t');
                string[] fileLine2Next = fileLine2ContentsNextFile.Split('\t');
                // If Vessel Name or Voyage do not match up,
                if (!(fileLine2Iter[INDEX_VESSELNAME].Equals(fileLine2Next[INDEX_VESSELNAME]) &&
                    fileLine2Iter[INDEX_VOYAGE].Equals(fileLine2Next[INDEX_VOYAGE])))
                {
                    return false;
                }
                filesContentsIterator--;
            }
            // All files Vessel name and voyage id are the same, return true
            return true;
        }

        private string[] ConvertToBayRowTier(string stowLocation)
        {
            stowLocation = stowLocation.Replace(" ", String.Empty);
            // If stow location is not string of 6 characters,
            if (stowLocation.Length != 6)
            {
                // return null value
                return null;
            }
            // Otherwise
            else
            {
                // Split string into string array of bay/row/tier
                string bay = stowLocation.Substring(0, 2);
                string row = stowLocation.Substring(2, 2);
                string tier = stowLocation.Substring(4, 2);

                string[] stowLocationParts = { bay, row, tier };
                return stowLocationParts;
            }
        }
    }
}