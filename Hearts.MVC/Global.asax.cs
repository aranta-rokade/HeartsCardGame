﻿using Hearts.MVC.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Hearts.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Register AuditTrail Attribute
            GlobalFilters.Filters.Add(new AuditTrailFilter());
            //Remove X-AspNetMvc-Version Response Header
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PreSendRequestHeaders()
        {
            //Remove Server Header   
            Response.Headers.Remove("Server");
            //Remove X-AspNet-Version Header
            Response.Headers.Remove("X-AspNet-Version");
        }
    }
}
