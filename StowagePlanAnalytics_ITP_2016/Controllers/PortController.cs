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
            // ------------------------------------------------Array -----------------------

            //// Start a new stopwatch timer.

            ////find out what the type in the textbox.this line will find the html elemet with
            ////the name portName and find its value.it will then store it as a lowercase string
            //string searchValue = Request.Params["portName"];
            ////to see what kind of button is pressed
            //string searchType = Request.Params["btnSearch"];
            //// Retrieve all ports from database
            //var allPorts = portGateway.SelectAll();
            ////if normal search button pressed
            //if (searchType == "portSearch")
            //{
            //    // Start a new stopwatch timer.
            //    timePerParse = Stopwatch.StartNew();
            //    // Retrieve all ports from database
            //    Port[] testSearch = portGateway.SelectAllArray();

            //    var testPort = portGateway.SelectAll();

            //    //if normal search button pressed
            //    SortedDictionary<string, Port> portAll = new SortedDictionary<string, Port>();

            //    foreach (var port in testPort)
            //    {
            //        portAll[port.PortCode] = port;
            //    }

            //    List<Port> searchResultList = new List<Port>();
            //    //if (portAll.ContainsKey(searchValue))
            //    //{
            //    //    searchResultList.Add(portAll[searchValue]);
            //    //}

            //    //initalizing a list to store all the ports for the search results

            //    //iterating through each port
            //    timePerParse = Stopwatch.StartNew();



            //    for (int i = 0; i < testSearch.Length; i++)
            //    {
            //        if (testSearch[i].PortCode.Contains(searchValue))
            //        {
            //            searchResultList.Add(testSearch[i]);
            //        }
            //    }



            //    timePerParse.Stop();
            //    Debug.WriteLine("DATA STRUCTURE: ARRAY");
            //    Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            //    Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            //    Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            //    return View("Index", searchResultList);

                //------------------------------------------- HASH MAP ---------------------------------------------


                //find out what the type in the textbox.this line will find the html elemet with
                //the name portName and find its value.it will then store it as a lowercase string
                string searchValue = Request.Params["portName"];
                //to see what kind of button is pressed
                string searchType = Request.Params["btnSearch"];
                // Retrieve all ports from database
                var allPorts = portGateway.SelectAll();
                //if normal search button pressed
                if (searchType == "portSearch")
                {
                    // Start a new stopwatch timer.
                    timePerParse = Stopwatch.StartNew();
                    // Retrieve all ports from database
                    Port[] testSearch = portGateway.SelectAllArray();

                    var testPort = portGateway.SelectAll();

                    //if normal search button pressed
                    SortedDictionary<string, Port> portAll = new SortedDictionary<string, Port>();

                    foreach (var port in testPort)
                    {
                        portAll[port.PortCode] = port;
                    }

                    List<Port> searchResultList = new List<Port>();
                    timePerParse = Stopwatch.StartNew();


                    if (portAll.ContainsKey(searchValue))
                    {
                        searchResultList.Add(portAll[searchValue]);
                    }


                    timePerParse.Stop();
                    Debug.WriteLine("DATA STRUCTURE: HASH MAP");
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
        public ActionResult search(string searchValue)
        {
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();
            // Retrieve all ports from database
            Port[] testSearch = portGateway.SelectAll().Cast<Port>().ToArray();
            //if normal search button pressed

            //initalizing a list to store all the ports for the search results
            List<Port> searchResultList = new List<Port>();
            //iterating through each port
            for (int i = 0; i < testSearch.Length; i++)
            {
                if (testSearch[i].PortName == searchValue)
                {
                    searchResultList.Add(testSearch[i]);
                }
            }
            //foreach (var port in allPorts)
            //{
            //    //lowercasing port name and port code
            //    string portName = port.PortName.ToLower();
            //    string portCode = port.PortCode.ToLower();

            //    //checking to see if either the port code or port name contains what the user has input
            //    if (portName.Contains(searchValue) || portCode.Contains(searchValue))
            //    {
            //        //insert into the list inialzed above
            //        searchResultList.Add(port);
            //    }
            //}


            timePerParse.Stop();
            Debug.WriteLine("DATA STRUCTURE: ARRAY");
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return View("Index", searchResultList);
        }
        [HttpPost]
        public ActionResult sort()
        {
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();
            // Retrieve all ports from database
            var allPorts = portGateway.SelectAll();
            //if normal search button pressed
            string sortType = Request.Params["btnSearch"];
            //initalizing a list to store all the ports for the search results
            List<Port> searchResultList = new List<Port>();
            if (sortType == "portSortAZ")
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
            else if (sortType == "portSortZA")
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
                Debug.WriteLine("BUBBLE SORT");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allPortList);
            }
            else if (sortType == "portSortMerge")
            {
                // Start a new stopwatch timer.
                timePerParse = Stopwatch.StartNew();
                // Retrieve all ports from database
                Port[] allPortsTest = portGateway.SelectAll().Cast<Port>().ToArray(); ;
                //if normal search button pressed
                MergeSort_Recursive(allPortsTest, 0, allPortsTest.Length - 1);

                timePerParse.Stop();
                Debug.WriteLine("MERGE SORT");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allPortsTest);
            }
            else if (sortType == "portSearch")
            {
                // Start a new stopwatch timer.
                timePerParse = Stopwatch.StartNew();
                // Retrieve all ports from database
                Port[] allPortsTest = portGateway.SelectAll().Cast<Port>().ToArray(); ;
                //if normal search button pressed
                MergeSort_Recursive(allPortsTest, 0, allPortsTest.Length - 1);

                timePerParse.Stop();
                Debug.WriteLine("MERGE SORT");
                Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
                Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
                Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
                return View("Index", allPortsTest);
            }
            //return view with the list if none of the buttons pressed
            timePerParse.Stop();
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return View("Index", allPorts);
        }
        public ActionResult sortMerge()
        {
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();
            // Retrieve all ports from database
            Port[]  allPorts = portGateway.SelectAll().Cast<Port>().ToArray(); ;
            //if normal search button pressed
            MergeSort_Recursive(allPorts, 0, allPorts.Length - 1);

            timePerParse.Stop();
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return View("Index", allPorts);
        }
        public ActionResult searchsortAZ(string searchValue)
        {
            // Start a new stopwatch timer.
            timePerParse = Stopwatch.StartNew();
            // Retrieve all ports from database
            var allPorts = portGateway.SelectAll();
            //if normal search button pressed

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
            //storing a temp port object for comparison later
            Port temp = null;

            //BUBBLE SORT
            for (int i = 0; i < searchResultList.Count; i++)
            {
                Port port = searchResultList[i];
                for (int sort = 0; sort < searchResultList.Count() - 1; sort++)
                {
                    if (searchResultList[sort].PortName[0] > searchResultList[sort + 1].PortName[0])
                    {
                        temp = searchResultList[sort + 1];
                        searchResultList[sort + 1] = searchResultList[sort];
                        searchResultList[sort] = temp;
                    }
                }
            }

            timePerParse.Stop();
            Debug.WriteLine("Time elapsed (s): {0}", timePerParse.Elapsed.TotalSeconds);
            Debug.WriteLine("Time elapsed (ms): {0}", timePerParse.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Time elapsed (ns): {0}", timePerParse.Elapsed.TotalMilliseconds * 1000000);
            return View("Index", searchResultList);

        }
        static public void DoMerge(Port[] allPorts, int left, int mid, int right)
        {
            Port[] temp = new Port[10033];
            int i, left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (allPorts[left].PortName[0] >= allPorts[mid].PortName[0])
                    temp[tmp_pos++] = allPorts[left++];
                else
                    temp[tmp_pos++] = allPorts[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = allPorts[left++];

            while (mid <= right)
                temp[tmp_pos++] = allPorts[mid++];

            for (i = 0; i < num_elements; i++)
            {
                allPorts[right] = temp[right];
                right--;
            }
        }

        static public void MergeSort_Recursive(Port[] allPorts, int left, int right)
        {
            int mid;

            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSort_Recursive(allPorts, left, mid);
                MergeSort_Recursive(allPorts, (mid + 1), right);

                DoMerge(allPorts, left, (mid + 1), right);
            }
        }

        public static void Quicksort(IComparable[] elements, int left, int right)
        {
            int i = left, j = right;
            IComparable pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        }
    }
}