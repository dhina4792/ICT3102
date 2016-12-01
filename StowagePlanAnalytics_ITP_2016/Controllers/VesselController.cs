using StowagePlanAnalytics_ITP_2016.DAL;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{


    public class VesselController : Controller
    {
        private VesselGateway vesselGateway = new VesselGateway();
        Stopwatch timePerParse;
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
                    return View();
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



        [HttpPost]
        public ActionResult sort()
        {
            // Start a new stopwatch timer.

            var allVesselsReturn = vesselGateway.SelectAll();
            //if normal search button pressed
            string sortType = Request.Params["btnSort"];

            if (sortType == "vesselCodeBubbleSortAZ")
            {
                //storing a temp port object for comparison later
                Vessel temp = null;
                // Retrieve all ports from database
                var allVessels = vesselGateway.SelectAll();
                List<Vessel> allVesselList = allVessels.ToList();
                //converting the ineurmable allPorts to a list for bubble sort

                timePerParse = Stopwatch.StartNew();
                //BUBBLE SORT
                for (int i = 0; i < allVesselList.Count; i++)
                {
                    Vessel port = allVesselList[i];
                    for (int sort = 0; sort < allVesselList.Count() - 1; sort++)
                    {
                        if (allVesselList[sort].VesselName[0] > allVesselList[sort + 1].VesselName[0])
                        {
                            temp = allVesselList[sort + 1];
                            allVesselList[sort + 1] = allVesselList[sort];
                            allVesselList[sort] = temp;
                        }
                    }
                }
                //return view with the list
                timePerParse.Stop();
                Debug.WriteLine("BUBBLE SORT A-Z");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allVesselList);
            }
            //if sort Z-A button pressed
            else if (sortType == "vesselCodeBubbleSortZA")
            {
                //storing a temp port object for comparison later
                Vessel temp = null;
                // Retrieve all ports from database
                var allVessels = vesselGateway.SelectAll();
                List<Vessel> allVesselList = allVessels.ToList();
                //BUBBLE SORT
                timePerParse = Stopwatch.StartNew();
                for (int i = 0; i < allVesselList.Count; i++)
                {
                    Vessel port = allVesselList[i];
                    for (int sort = 0; sort < allVesselList.Count() - 1; sort++)
                    {
                        if (allVesselList[sort].VesselName[0] < allVesselList[sort + 1].VesselName[0])
                        {
                            temp = allVesselList[sort + 1];
                            allVesselList[sort + 1] = allVesselList[sort];
                            allVesselList[sort] = temp;
                        }
                    }
                }
                //return view with the list
                timePerParse.Stop();
                Debug.WriteLine("BUBBLE SORT Z-A");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allVesselList);
            }
            else if (sortType == "vesselCodeSortMergeZA")
            {            // Retrieve all ports from database
                var allVessels = vesselGateway.SelectAll();
                // Start a new stopwatch timer.
                timePerParse = Stopwatch.StartNew();

                // Retrieve all ports from database
                Vessel[] allVesselsArray = allVessels.Cast<Vessel>().ToArray(); ;
                //if normal search button pressed
                MergeSort_Recursive(allVesselsArray, 0, allVesselsArray.Length - 1);

                timePerParse.Stop();
                Debug.WriteLine("MERGE SORT");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allVesselsArray);
            }

            return View("Index", allVesselsReturn);
        }

        static public void DoMerge(Vessel[] allVessels, int left, int mid, int right)
        {
            Vessel[] temp = new Vessel[10004];
            int i, left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (allVessels[left].VesselName[0] >= allVessels[mid].VesselName[0])
                    temp[tmp_pos++] = allVessels[left++];
                else
                    temp[tmp_pos++] = allVessels[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = allVessels[left++];

            while (mid <= right)
                temp[tmp_pos++] = allVessels[mid++];

            for (i = 0; i < num_elements; i++)
            {
                allVessels[right] = temp[right];
                right--;
            }
        }

        static public void MergeSort_Recursive(Vessel[] allVessels, int left, int right)
        {
            int mid;

            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSort_Recursive(allVessels, left, mid);
                MergeSort_Recursive(allVessels, (mid + 1), right);

                DoMerge(allVessels, left, (mid + 1), right);
            }
        }
    }
}