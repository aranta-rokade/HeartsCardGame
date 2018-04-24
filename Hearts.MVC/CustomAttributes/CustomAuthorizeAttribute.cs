using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hearts.MVC.CustomAttributes
{
    public class CustomAuthorizeAttribute: ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string TempSession =
                    Convert.ToString(filterContext.HttpContext.Session["AuthenticationToken"]);
            string TempAuthCookie =
            Convert.ToString(filterContext.HttpContext.Request.Cookies["AuthenticationToken"].Value);

            if (TempSession != null && TempAuthCookie != null)
            {
                if (!TempSession.Equals(TempAuthCookie))
                {
                    ViewResult result = new ViewResult();
                    result.ViewName = "Login";
                    filterContext.Result = result;
                }
            }
            else
            {
                ViewResult result = new ViewResult();
                result.ViewName = "Login";
                filterContext.Result = result;
            }
        }
    }
}