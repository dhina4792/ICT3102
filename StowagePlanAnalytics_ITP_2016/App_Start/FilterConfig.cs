﻿using System.Web;
using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
