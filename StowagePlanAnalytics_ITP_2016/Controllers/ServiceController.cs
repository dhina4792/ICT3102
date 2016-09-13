using StowagePlanAnalytics_ITP_2016.DAL;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ServiceController : Controller
    {
        ServiceGateway serviceGateway = new ServiceGateway();

        // GET: /Admin/Service
        public ActionResult Index()
        {
            var model = serviceGateway.SelectAll();
            return View(model);
        }

        // GET: /Admin/Service
        public ActionResult Create()
        {
            List<SelectListItem> objResult = new List<SelectListItem>();

            // Retrieve all ports
            var ports = new PortGateway().SelectAll();

            foreach (var port in ports)
            {
                objResult.Add(new SelectListItem() { Text = port.PortCode, Value = port.PortCode });
            }

            ViewBag.PortCodes = objResult;

            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];

            return View();
        }

        //Teck Loon: Retrieve port name that matches the port code and return as json
        [HttpPost]
        public ActionResult Check(string PortCode)
        {
            DAL.CRUDGateway<Port> gw = new DAL.CRUDGateway<Port>();
            Port resultPort = gw.SelectByPrimaryKey(PortCode); //Check by Primary key for relevant result

            return Json(new { PortCode = resultPort.PortCode, PortName = resultPort.PortName });
        }

        //Teck Loon: Retrieve list of port attach to the service code and return as json
        [HttpPost]
        public ActionResult Retrieve(string ServiceCode)
        {
            Service resultService = serviceGateway.SelectByPrimaryKey(ServiceCode);

            return Json(new { ServiceCode = resultService.ServiceCode, PortList = resultService.Ports });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceCode")] Service service, string[] PortCode, string[] PortName, bool[] FileUpload) 
        {
            List<Port> ports = new List<Port>();
            if (ModelState.IsValid) //Check for validation issues on server for user inputs
            {
                // If service code not in use,
                if (serviceGateway.SelectByPrimaryKey(service.ServiceCode) == null)
                {
                    for (int i = 0; i < PortCode.Length; i++) //Loop to add all port into the list
                    {
                        var port = new Port();
                        port.PortCode = PortCode[i];
                        port.PortName = PortName[i];
                        port.SequenceNo = i + 1;
                        port.FileUpload = FileUpload[i];
                        ports.Add(port);
                    }
                    service.Ports = ports;

                    // Create Service
                    try
                    {
                        serviceGateway.Insert(service);
                        //Display Success Message
                        TempData["Status"] = "Success";
                        TempData["Message"] = "Service " + service.ServiceCode + " was successfully created in the system."; //Saved Error Message
                        ModelState.Clear();
                    }
                    catch (NullReferenceException)
                    {
                        // One or more input parameters are null
                        TempData["Status"] = "Fail";
                        TempData["Message"] = "Failed to create Service " + service.ServiceCode + ". One or more input parameters are missing.";
                    }
                    catch (Exception)
                    {
                        TempData["Status"] = "Fail";
                        TempData["Message"] = "Failed to create Service " + service.ServiceCode + ". One or more input parameters contain invalid values.";
                    }

                    return RedirectToAction("Create");
                }
                else
                {
                    //Display Fail Message
                    TempData["Status"] = "Fail";
                    TempData["Message"] = "Service " + service.ServiceCode + " already exists in the system."; //Saved Error Message

                    return RedirectToAction("Create");
                }
            }
            else
            {
                // Model Validation failed
                TempData["Status"] = "Fail";
                TempData["Message"] = "One or more input fields contain invalid values.";
                return RedirectToAction("Create");
            }
        }

        // GET: /Admin/Service/Delete
        public ActionResult Delete(string serviceCode)
        {
            if (serviceCode == null)
            {
                ViewBag.ModelNullMessage = "Service Code does not exist.";
                return PartialView(null);
            }
            Service service = serviceGateway.SelectByPrimaryKey(serviceCode);

            if (service == null)
            {
                ViewBag.ModelNullMessage = "Service Code " + serviceCode + " does not exist.";
            }
            return PartialView("Delete", service);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string serviceCode)
        {
            try
            {
                serviceGateway.Delete(serviceCode);
                ViewBag.Status = "Success";
                ViewBag.Message = "Service " + serviceCode + " deleted.";
                return PartialView("DeleteConfirmed");
            }
            catch (Exception)
            {
                ViewBag.Message = "Something went wrong with the deletion process. Please try again.";
                return PartialView("DeleteConfirmed");
            }
        }
    }
}