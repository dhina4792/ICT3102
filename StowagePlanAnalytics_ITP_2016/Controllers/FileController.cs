using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using StowagePlanAnalytics_ITP_2016.Models.FileModel;
using StowagePlanAnalytics_ITP_2016.Models;
using StowagePlanAnalytics_ITP_2016.DAL;
using StowagePlanAnalytics_ITP_2016.Calculations;
using System.Threading;
using System.Diagnostics;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        DataGateway gw = new DataGateway();
        Stopwatch timePerParse;
        // GET: File
        public ActionResult Index()
        {
            return RedirectToAction("Upload");
        }

        public ActionResult _PartialViewServiceUploadFiles(string serviceCode)
        {
            // Retrieve Data from database
            ServiceGateway serviceGateway = new ServiceGateway();
            //var service = serviceGateway.SelectByPrimaryKey(serviceCode);
            var service = gw.GetService(serviceCode);

            // If service not found in database
            if (service == null)
                throw new Exception();  // Return error page? TODO: implement error partial view

            // Arrange ports by sequence
            service.Ports.OrderBy(p => p.SequenceNo);

            // Pass data to view
            return PartialView(service.Ports.Where(p => p.FileUpload == true));
        }

        // GET: File
        public ActionResult Upload(string uploadResult, string fileName)
        {
            if (uploadResult == "inputError")
            {
                ViewBag.Status = "Error";
                ViewBag.Message = "Input parameters do not match.";
            }
            else if (uploadResult == "missingFile")
            {
                ViewBag.Status = "Error";
                ViewBag.Message = "One or more files not uploaded.";
            }
            else if (uploadResult == "processError")
            {
                ViewBag.Status = "Error";
                ViewBag.Message = "Failed to process files, please make sure the correct STIF files are uploaded!";
            }
            else if (uploadResult == "fileError")
            {
                ViewBag.Status = "Error";
                ViewBag.Message = "Failed to process file: " + fileName;
            }
            else if (uploadResult == "success")
            {
                ViewBag.Status = "Success";
                ViewBag.Message = "STIF file successfully uploaded!";
            }

            // Get all service names from database, return type: IEnumerable<Service>
            CRUDGateway<Service> serviceGateway = new CRUDGateway<Service>();
            var services = serviceGateway.SelectAll();

            // Create list of select list items
            List<SelectListItem> servicesSelectList = new List<SelectListItem>();

            foreach (var service in services)
            {
                servicesSelectList.Add(new SelectListItem { Text = service.ServiceCode, Value = service.ServiceCode });
            }



            // Pass Services Select List to View
            return View(servicesSelectList);
        }

        // POST: File
        // Logic to process file HERE
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] file, int[] sequenceno, string ServiceDropdownlist)
        {
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();
            // Verify valid number of files and corresponding sequenceno
            if (file.Length != sequenceno.Length)
            {

                return RedirectToAction("Upload", "File", new { uploadResult = "inputError" });
            }

            // Verify null files
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == null)
                {
                    return RedirectToAction("Upload", "File", new { uploadResult = "missingFile" });
                }
            }
            // Retrieve Data from database

            // Convert HttpPostedFileBase object(s) to UploadedFile
            var uploadedFiles = FileTranslater.HttpPostedFileBaseToUploadedFile(file);

            try
            {
                Service svc = gw.GetService(ServiceDropdownlist);
                if (svc == null)
                {
                    throw new Exception("Service " + ServiceDropdownlist + " not found.");
                }

                FileProcesser.ProcessFiles(svc, uploadedFiles, sequenceno);
            }
            catch (Exception ex)
            {
                if (ex.Message.Split(':')[0] == "File")
                {
                    ViewBag.Message = "Error: Failed to process file - " + ex.Message.Split(':')[1];
                    return RedirectToAction("Upload", "File", new { uploadResult = "fileError", fileName = ex.Message.Split(':')[1] });
                }
                else
                {
                    ViewBag.Message = "Error: " + ex.Message;
                    return RedirectToAction("Upload", "File", new { uploadResult = "processError" });
                }
            }
            timePerParse.Stop();
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return RedirectToAction("Upload", "File", new { uploadResult = "success" });
        }

        // Returns a list of SelectListItem objects.
        // These objects are going to be used later in the upload view to render the DropDownList.
        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            // <option value="Service Name">Service Name</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }

            return selectList;
        }


    }
}