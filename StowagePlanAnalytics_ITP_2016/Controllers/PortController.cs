using StowagePlanAnalytics_ITP_2016.Models;
using StowagePlanAnalytics_ITP_2016.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class PortController : Controller
    {
        private PortGateway portGateway = new PortGateway();
        Stopwatch timePerParse;

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


        [HttpPost]
        public ActionResult searchPort(FormCollection collection)
        {   
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();

            //find out what the type in the textbox.this line will find the html elemet with
            //the name portName and find its value.it will then store it as a lowercase string
            string searchValue = Request.Params["portName"].ToLower();

            //to see what kind of button is pressed
            string searchType = Request.Params["btnSearch"];

            // Retrieve all ports from database
            var allPorts = portGateway.SelectAll();

            //if normal search button pressed
            if (searchType == "portSearch")
            {
                //initalizing a list to store all the ports for the search results
                List<Port> searchResultList = new List<Port>();
                //iterating through each port
                foreach (var port in allPorts)
                {
                    //lowercasing port name and port code
                    string portName = port.PortName.ToLower();
                    string portCode = port.PortCode.ToLower();

                    //checking to see if either the port code or port name contains what the user has input
                    if (portName.Contains(searchValue) || portCode.Contains(searchValue))
                    {
                        //insert into the list inialzed above
                        searchResultList.Add(port);
                    }
                }
                timePerParse.Stop();
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", searchResultList);


            }
            //if sort alphabeticlly button pressed
            else if (searchType == "portSortAZ")
            {
                //storing a temp port object for comparison later
                Port temp = null;

                //converting the ineurmable allPorts to a list for bubble sort
                List<Port> allPortList = allPorts.ToList();

                //BUBBLE SORT
                for (int i = 0; i < allPortList.Count; i++)
                {
                    Port port = allPortList[i];
                    for (int sort = 0; sort < allPortList.Count() - 1; sort++)
                    {
                        if (allPortList[sort].PortName[0] > allPortList[sort + 1].PortName[0])
                        {
                            temp = allPortList[sort + 1];
                            allPortList[sort + 1] = allPortList[sort];
                            allPortList[sort] = temp;
                        }
                    }
                }
                //return view with the list
                timePerParse.Stop();
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allPortList);
            }
            //if sort Z-A button pressed
            else if (searchType == "portSortZA")
            {
                //storing a temp port object for comparison later
                Port temp = null;

                //converting the ineurmable allPorts to a list for bubble sort
                List<Port> allPortList = allPorts.ToList();

                //BUBBLE SORT(reverse)
                for (int i = 0; i < allPortList.Count; i++)
                {
                    Port port = allPortList[i];
                    for (int sort = 0; sort < allPortList.Count() - 1; sort++)
                    {
                        if (allPortList[sort].PortName[0] < allPortList[sort + 1].PortName[0])
                        {
                            temp = allPortList[sort + 1];
                            allPortList[sort + 1] = allPortList[sort];
                            allPortList[sort] = temp;
                        }
                    }
                }
                //return view with the list
                timePerParse.Stop();
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allPortList);
            }
            //return view with the list if none of the buttons pressed
            timePerParse.Stop();
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return View("Index", allPorts);
        }

    }
}