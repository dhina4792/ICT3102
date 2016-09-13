using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StowagePlanAnalytics_ITP_2016.DAL;
using System.Threading;
using StowagePlanAnalytics_ITP_2016.Calculations;

namespace StowagePlanAnalytics_ITP_2016.Models.FileModel
{
    public class FileProcesser
    {
        public static void ProcessFiles(Service service, UploadedFile[] files, int[] filesPortSequence, int[] tripID = null)
        {
            try
            {
                // Call translation function, returns translated model
                object translatedModel = FileTranslater.TranslateFiles(files);
                // Get type of model returned by function
                Type translatedModelType = translatedModel.GetType();
                if (translatedModelType == typeof(Voyage))
                {
                    Voyage voyage = (Voyage)translatedModel;

                    // Place Service Code into Voyage
                    voyage.ServiceCode = service.ServiceCode;

                    //retrieve vessel's details and each port details from database for calculations
                    voyage.Vessel = new DataGateway().GetVesselDetails(voyage.VesselName);
                    int i = 0;
                    foreach (var trip in voyage.Trips)
                    {
                        trip.DepPort =
                            service.Ports
                            .Where(p => p.PortCode.Equals(trip.DeparturePort) && p.SequenceNo == filesPortSequence[i]).First();
                        // ServicePort sequence number starts from 1, no need to +1 to get next port
                        trip.ArrivalPort = service.Ports.ElementAt(trip.DepPort.SequenceNo % service.Ports.Count()).PortCode;
                        i++;
                    }
                    // Call calculation routine here (typecast to Voyage before passing as parameter) on new thread
                    // run your code here
                    CalculationOptimized calculation = new CalculationOptimized(voyage);
                    List<UsefulInfo> listOfUsefulInfoModels = calculation.listOfUsefulInfoModels;

                    // Update UsefulInfo Id
                    if (tripID != null)
                    {
                        int y = 0;
                        foreach (var usefulInfo in listOfUsefulInfoModels)
                        {
                            usefulInfo.TripID = tripID[y];
                            y++;
                        }
                    }


                    // Store file in database and store useful info
                    new DataGateway().SaveFileAndCalculations(voyage, files, listOfUsefulInfoModels.ToArray());
                }
                else
                {
                    // Add other type checking here,
                    // or handle unknown translated model
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                string fileIndex = message.Split(';')[1];
                int n;
                if (Int32.TryParse(fileIndex, out n))
                {
                    string filename = files[n].FileName;
                    throw new Exception("File:" + filename);
                }
            }
        }
    }
}