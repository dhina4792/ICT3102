using StowagePlanAnalytics_ITP_2016.Calculations;
using System.Web.Mvc;
using StowagePlanAnalytics_ITP_2016.DAL;
using System.Collections.Generic;
using System.Linq;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Linq.Expressions;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize]
    public class DataAnalysisController : Controller
    {
        DataGateway gw = new DataGateway();
        // GET: DataAnalysis

        public ActionResult DataAnalysis()
        {

            ViewBag.Services = new SelectList(gw.GetServiceList());
            ViewBag.Classes = new SelectList(gw.GetClassList());

            return View();
        }

        public ActionResult FillDepartPort(string serviceCode)
        {
            DataGateway gw = new DataGateway();
            var ports = gw.GetPortList(serviceCode);
            return Json(ports, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RetrieveAllPort()
        {
            DataGateway gw = new DataGateway();
            var ports = gw.GetAllPortList();
            return Json(ports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            string serviceCode = collection.Get("ServiceCode");
            string VesselTEUClassCode = collection.Get("VesselTEUClassCode");
            string departPort = collection.Get("DepPortCode");
            string arrivalPort = collection.Get("ArrPortCode");
            string VoyageID = collection.Get("VoyageID");
            string searchValue = Request.Params["btnSearch"];
            IEnumerable<UsefulInfo> usefulInfoList = null;

            if (serviceCode == "" && VesselTEUClassCode != "" && VoyageID != "")
            {
                if (searchValue == "search")
                {
                    usefulInfoList = gw.search(collection);
                    TempData["searchCheck"] = "Voyage"; //Check if it is voyage search
                    TempData["searchCount"] = usefulInfoList.Count(); //Check if it is voyage search
                    TempData["searchVoyageID"] = VoyageID; //Saved Search Count
                }
                else if (searchValue == "advanceSearch")
                {
                    if (gw.validateAdvanceSearchCriteria(collection))
                    {
                        usefulInfoList = gw.advanceSearch(collection);
                        TempData["searchCheck"] = "Voyage"; //Check if it is voyage search
                        TempData["searchCount"] = usefulInfoList.Count(); //Check if it is voyage search
                        TempData["searchVoyageID"] = VoyageID; //Saved Search Count
                    }
                    else
                    {
                        TempData["searchCount"] = "invalidParameters";
                    }
                }
            }
            else if (VesselTEUClassCode != "")
            {
                if (searchValue == "search")
                {
                    usefulInfoList = gw.search(collection);
                    TempData["searchCount"] = usefulInfoList.Count(); //Saved Search Count
                    TempData["searchService"] = serviceCode; //Saved Search Count
                }
                else if (searchValue == "advanceSearch")
                {
                    if (gw.validateAdvanceSearchCriteria(collection))
                    {
                        usefulInfoList = gw.advanceSearch(collection);
                        TempData["searchCount"] = usefulInfoList.Count(); //Saved Search Count
                        TempData["searchService"] = serviceCode; //Saved Search Count
                    }
                    else
                    {
                        TempData["searchCount"] = "invalidParameters";
                    }
                }
            }
            else
            {
                TempData["searchCount"] = "invalidParameters"; //Saved Search Count
            }
            
            ViewBag.Services = gw.GetServiceListItems(serviceCode);
            ViewBag.Classes = gw.GetClassListItems(VesselTEUClassCode);
            ViewBag.VoyageID = VoyageID;
            /*To get the previous search departPort and DestPort*/
            ViewBag.DepartPort = departPort;
            ViewBag.DestPort = arrivalPort;
            return View("DataAnalysis", usefulInfoList);
        }


    }
}