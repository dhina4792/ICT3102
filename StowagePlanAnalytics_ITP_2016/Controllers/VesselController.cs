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
    public class VesselController : Controller
    {
        private VesselGateway vesselGateway = new VesselGateway();

        // GET: /Admin/Vessel
        public ActionResult Index()
        {
            var model = vesselGateway.SelectAll();

            return View(model);
        }

        // GET: /Admin/Vessel/Create
        public ActionResult Create()
        {
            SetVesselTEUClassCodeViewBag();

            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VesselCode, VesselName, VesselTEUClassCode, TEUCapacity, MaxWeight, MaxReefers")] Vessel vessel)
        {
            SetVesselTEUClassCodeViewBag(vessel.VesselTEUClassCode);

            if (ModelState.IsValid) //Check for validation issues on server for user inputs
            {
                if (vesselGateway.SelectByPrimaryKey(vessel.VesselCode) == null)
                {
                    // save to db
                    try
                    {
                        vesselGateway.Insert(vessel);
                        TempData["Status"] = "Success";
                        TempData["Message"] = "Vessel " + vessel.VesselCode + " was successfully created in the system.";
                        return RedirectToAction("Create");
                    }
                    catch (Exception)
                    {
                        // One of the foreign key constraints failed
                        ViewBag.Status = "Fail";
                        ViewBag.Message = "One or more input fields contain invalid values";
                        return View();
                    }
                }
                else
                {
                    //Display Error for duplicate VesselCode
                    ViewBag.Status = "Fail";
                    ViewBag.Message = "Vessel " + vessel.VesselCode + " already exists in the system."; //Saved Error Message
                    return View() ;
                }
            }
            else
            {
                ViewBag.Status = "Fail";
                ViewBag.Message = "One or more input fields contain invalid values";
                return View();
            }
        }
        private void SetVesselTEUClassCodeViewBag(string selectedVesselTEUClassCode = null)
        {
            var gateway = new CRUDGateway<Class>();
            var classes = gateway.SelectAll();

            // Create SelectList from classes and set text and value to VesselTEUClassCode
            ViewBag.VesselClassDropList = new SelectList(classes, "VesselTEUClassCode", "VesselTEUClassCode", selectedVesselTEUClassCode);
        }

        // GET: /Admin/Vessel/Edit
        public ActionResult Edit(string vesselCode)
        {
            Vessel vessel = vesselGateway.SelectByPrimaryKey(vesselCode);
            
            SetVesselTEUClassCodeViewBag(vessel.VesselTEUClassCode);
            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            ViewData["storeInitialVesselCode"] = vessel.VesselCode;
            return PartialView("Edit", vessel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string beforeVesselValue, [Bind(Include = "VesselCode, VesselName, VesselTEUClassCode, TEUCapacity, MaxWeight, MaxReefers")] Vessel vessel)
        {
            SetVesselTEUClassCodeViewBag(vessel.VesselTEUClassCode);
            if (ModelState.IsValid)
            {
                //Check whether portcode is the same as before
                if (beforeVesselValue != vessel.VesselCode)
                {
                    if (vesselGateway.SelectByPrimaryKey(vessel.VesselCode) == null)
                    {
                        // save to db
                        vesselGateway.Update(vessel, beforeVesselValue);
                        
                        TempData["Status"] = "Success";
                        TempData["Message"] = "Vessel information updated.";
                        return RedirectToAction("Edit", new { vesselCode = vessel.VesselCode });
                    }
                    else
                    {
                        //Retrieve Value for dropdown list vessel class for dropdown list in Create Vessel Page.
                        ViewData["storeInitialVesselCode"] = beforeVesselValue;

                        //Existing Port in the System
                        ViewBag.Status = "Fail";
                        ViewBag.Message = "Unable to change vessel code to " + vessel.VesselCode + ". Vessel Code already exists in the system."; //Saved Error Message
                        return PartialView();
                    }
                }
                else
                {
                    //Save to DB if only information needed to be update.
                    vesselGateway.Update(vessel);
                    TempData["Status"] = "Success";
                    TempData["Message"] = "Vessel information updated.";
                    return RedirectToAction("Edit", new { vesselCode = vessel.VesselCode });
                }
            }
            else
            {
                //Retrieve Value for dropdown list vessel class for dropdown list in Create Vessel Page.
                ViewData["storeInitialVesselCode"] = vessel.VesselCode;
                ViewBag.Status = "Fail";
                ViewBag.Message = "One or more input fields contain invalid values.";
                return PartialView("Edit", vessel);
            }
        }

        // GET: /Admin/Vessel/Delete
        public ActionResult Delete(string vesselCode)
        {
            if (vesselCode == null)
            {
                ViewBag.ModelNullMessage = "Vessel Code does not exist.";
                return PartialView(null);
            }
            Vessel vessel = vesselGateway.SelectByPrimaryKey(vesselCode);
            if (vessel == null)
            {
                ViewBag.ModelNullMessage = "Vessel Code " + vesselCode + " does not exist.";
            }
            return PartialView("Delete", vessel);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string vesselCode)
        {
            try
            {
                if (vesselGateway.Delete(vesselCode) == null)
                {
                    ViewBag.Status = "Fail";
                    ViewBag.Message = "Vessel " + vesselCode + " does not exist.";
                }
                else
                {
                    ViewBag.Status = "Success";
                    ViewBag.Message = "Vessel " + vesselCode + " deleted.";
                }
                return PartialView("DeleteConfirmed");
            }
            catch (Exception)
            {
                ViewBag.Status = "Fail";
                ViewBag.Message = "Something went wrong with the deletion process. Please try again.";
                return PartialView("DeleteConfirmed");
            }
        }
    }
}