using StowagePlanAnalytics_ITP_2016.DAL;
using StowagePlanAnalytics_ITP_2016.Models.FileModel;
using StowagePlanAnalytics_ITP_2016.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class STIFFileManagementController : Controller
    {
        private DataGateway gw = new DataGateway();

        // GET: STIFFileManagement
        public ActionResult Index()
        {
            // Retrieve all voyages from database
            var voyages = gw.GetAllVoyages();

            // Order voyages by service code
            voyages = voyages.OrderBy(v => v.ServiceCode);
            // Initialize view model list
            var viewModelList = new List<VM_STIFFileManagement_Index>();
            // Initialize helper pointers
            string currentServiceCode = null;
            VM_STIFFileManagement_Index currentViewModelElement = null;
            List<VM_STIFFileManagement_Index.Voyage> currentViewModelElementVoyageList = null;
            foreach (var voyage in voyages)
            {
                if (!voyage.ServiceCode.Equals(currentServiceCode))
                {
                    //This code block will only execute for the first voyage element with a new service code
                    //This code block only changes pointer values

                    // Set current service code to this service code
                    currentServiceCode = voyage.ServiceCode;

                    // Create new list of voyages for view model element
                    currentViewModelElement = new VM_STIFFileManagement_Index();
                    currentViewModelElement.ServiceCode = currentServiceCode;
                    // Add current view model element to view model list
                    viewModelList.Add(currentViewModelElement);

                    // Create new list of view model voyage
                    currentViewModelElementVoyageList = new List<VM_STIFFileManagement_Index.Voyage>();
                    // Assign new list of view model voyage to current view model element
                    currentViewModelElement.Voyages = currentViewModelElementVoyageList;
                }
                // Create model voyage object
                var modelVoyageObject = new VM_STIFFileManagement_Index.Voyage();
                modelVoyageObject.VoyageID = voyage.VoyageID;
                // Add model voyage object to model element voyage list
                currentViewModelElementVoyageList.Add(modelVoyageObject);
            }
            return View(viewModelList);
        }

        // GET: STIFFileManagement/Details/5
        public ActionResult Details(string voyageID)
        {
            if (!String.IsNullOrWhiteSpace(voyageID))
            {
                // Retrieve STIF Files with voyage ID
                var viewModelList = gw.GetSTIFFilesByVoyageID(voyageID);
                // Get voyage's service code
                ViewBag.ServiceCode = new CRUDGateway<Models.Voyage>().SelectByPrimaryKey(voyageID).ServiceCode;
                return PartialView(viewModelList);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult ReplaceSTIFFiles(string serviceCode, HttpPostedFileBase[] file, int[] portSequence, int[] fileId, int[] tripId)
        {
            // Initialize Uploaded Files list
            var uploadedFilesList = new List<UploadedFile>();

            // For all form inputs,
            for (int i = 0; i < file.Length; i++)
            {
                // If file input does not have a new file for replacement,
                if (file[i] == null)
                {
                    // Retrieve file from database
                    uploadedFilesList.Add(gw.GetUploadedFileById(fileId[i]));
                }
                else
                {
                    // Convert file to Uploaded File type
                    var uploadedFile = new UploadedFile(file[i]);
                    uploadedFile.Id = fileId[i];
                    uploadedFilesList.Add(uploadedFile);
                    
                }
            }
            try
            {
                // Retrieve service
                var service = gw.GetService(serviceCode);

                // Process file
                FileProcesser.ProcessFiles(service, uploadedFilesList.ToArray(), portSequence, tripId);
                TempData["Redirect"] = true;
                TempData["Success"] = true;
                TempData["Message"] = "File(s) and Calculation(s) updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
                TempData["Message"] = "Error:" + ex.Message;
                //return RedirectToAction("Index");
                TempData["Redirect"] = true;
                TempData["Success"] = false;
                TempData["Message"] = "Failed to replace file(s). System Exception: " + ex.Message;
                return RedirectToAction("Index");
                
                ////TEST FORM SUBMIT
                //ViewBag.Success = false;
                //return PartialView("DeleteConfirmed");
            }
        }

        // GET: Admin/STIFFileManagement/Download/5
        public ActionResult Download(int? id)
        {
            if (id != null)
            {
                var fileGateway = new CRUDGateway<UploadedFile>();
                var file = fileGateway.SelectByPrimaryKey(id);

                string mimeType = "text/stif";

                return File(file.FileContent, mimeType, file.FileName);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        // GET: STIFFileManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: STIFFileManagement/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: STIFFileManagement/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: STIFFileManagement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: STIFFileManagement/Delete/5
        public ActionResult Delete(string voyageID)
        {
            if (String.IsNullOrWhiteSpace(voyageID))
            {
                ViewBag.VoyageID = null;
            }
            else
            {
                ViewBag.VoyageID = voyageID;
            }
            
            return PartialView();
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string voyageID)
        {
            if (gw.DeleteUploadedFilesByVoyageID(voyageID))
            {
                ViewBag.Success = true;
                ViewBag.voyageID = voyageID;
                return PartialView("DeleteConfirmed");
            }
            else
            {
                ViewBag.Success = false;
                return PartialView("DeleteConfirmed");
            }
        }
    }
}
