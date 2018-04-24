using System;
using System.Web.Mvc;

namespace Hearts.MVC.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CustomAuthorizeAttribute: ActionFilterAttribute, IAuthorizationFilter
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