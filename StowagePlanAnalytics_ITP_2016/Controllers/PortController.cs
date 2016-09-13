using StowagePlanAnalytics_ITP_2016.Models;
using StowagePlanAnalytics_ITP_2016.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class PortController : Controller
    {
        private PortGateway portGateway = new PortGateway();

        // GET: /Admin/Port
        public ActionResult Index()
        {
            // Retrieve all ports from database
            var model = portGateway.SelectAll();

            return View("Index", model);
        }

        // GET: /Admin/Port/Create
        public ActionResult Create()
        {
            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "PortCode, PortName, NoOfCranes, CostOfMove")] Port port)
        {
            if (ModelState.IsValid) //Check for validation issues on server for user inputs
            {
                // If Port Code does not exist in database,
                if (portGateway.SelectByPrimaryKey(port.PortCode) == null)
                {
                    // Create port and save to db
                    portGateway.Insert(port);
                    TempData["Status"] = "Success";
                    TempData["Message"] = "Port " + port.PortCode + " was successfully created in the system."; //Saved Error Message
                    ModelState.Clear();
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Status"] = "Fail";
                    TempData["Message"] = "Port " + port.PortCode + " already exists in the system."; //Saved Error Message
                    return RedirectToAction("Create");
                }
            }
            else
            {
                ViewBag.Status = "Fail";
                ViewBag.Message = "One or more input fields contain invalid values";
                return View();
            }
        }


        // GET: /Admin/Port/Edit
        public ActionResult Edit(string portCode)
        {
            var port = portGateway.SelectByPrimaryKey(portCode);

            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];

            ViewData["storeInitialPortCode"] = portCode;
            return PartialView("Edit", port);
        }
        [HttpPost]
        public ActionResult Edit(string beforePortValue, [Bind(Include = "PortCode, PortName, NoOfCranes, CostOfMove")] Port port)
        {
            if (ModelState.IsValid && beforePortValue != null) //Check for validation issues on server for user inputs
            {
                //Check whether portcode is the same as before
                if (beforePortValue != port.PortCode)
                {
                    // If Port Code does not exist in database,
                    if (portGateway.SelectByPrimaryKey(port.PortCode) == null)
                    {
                        // save to db
                        portGateway.Update(port, beforePortValue);
                        TempData["Status"] = "Success";
                        TempData["Message"] = "Port " + port.PortCode + " updated.";
                        return RedirectToAction("Edit", new { portCode = port.PortCode });
                    }
                    else
                    {
                        ViewData["storeInitialPortCode"] = beforePortValue;

                        //Existing Port in the System
                        TempData["Status"] = "Fail";
                        TempData["Message"] = "Unable to change port code to " + port.PortCode + "; Port Code already exists in the system."; //Saved Error Message
                        return RedirectToAction("Edit", new { portCode = beforePortValue });
                    }
                }
                else
                {
                    //Save to DB if only information needed to be update.
                    portGateway.Update(port);
                    TempData["Status"] = "Success";
                    TempData["Message"] = "Port " + port.PortCode + " updated.";
                    return RedirectToAction("Edit", new { portCode = port.PortCode });
                }

            }
            else
            {
                ViewBag.Status = "Fail";
                ViewBag.Message = "One or more input fields contain invalid values.";
                return PartialView("Edit", port);
            }
        }

        // GET: /Admin/Port/Delete
        public ActionResult Delete(string portCode)
        {
            if (portCode == null)
            {
                ViewBag.ModelNullMessage = "Port Code does not exist.";
                return PartialView("Delete", null);
            }
            Port port = portGateway.SelectByPrimaryKey(portCode);
            if (port == null)
            {
                ViewBag.ModelNullMessage = "Port Code " + portCode + " does not exist.";
            }
            return PartialView("Delete", port);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string portCode)
        {
            try
            {
                portGateway.Delete(portCode);
                ViewBag.Status = "Success";
                ViewBag.Message = "Port " + portCode + " deleted.";
                return PartialView("DeleteConfirmed");
            }
            catch (Exception)
            {
                // Assume exception generated due to relation table ServicePort constraint 
                // on Port Primary Key, configured as ON DELETE RESTRICT
                ViewBag.Status = "Fail";
                ViewBag.Message = "Unable to delete Port " + portCode + ". Port is in use by one or more Services.";
                return PartialView("DeleteConfirmed");
            }
        }
    }
}