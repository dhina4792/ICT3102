using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StowagePlanAnalytics_ITP_2016
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Port",
                url: "Admin/Port/{action}/{id}",
                defaults: new { controller = "Port", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Vessel",
                url: "Admin/Vessel/{action}/{id}",
                defaults: new { controller = "Vessel", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Service",
                url: "Admin/Service/{action}/{id}",
                defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LogData",
                url: "Admin/LogData/{action}/{id}",
                defaults: new { controller = "LogData", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "STIFFileManagement",
                url: "Admin/STIFFileManagement/{action}/{id}",
                defaults: new { controller = "STIFFileManagement", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
