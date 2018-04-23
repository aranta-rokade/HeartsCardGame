﻿using Hearts.MVC.CustomAttributes;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
