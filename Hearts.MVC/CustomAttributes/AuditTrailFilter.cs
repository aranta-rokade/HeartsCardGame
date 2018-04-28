using Hearts.BAL;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC.CustomAttributes
{
    public class AuditTrailFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AuditTrailBAL a_bal = new AuditTrailBAL();
            AuditTrailModel objaudit = new AuditTrailModel();
            //Getting Action Name
            string actionName = filterContext.ActionDescriptor.ActionName;
            //Getting Controller Name
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var request = filterContext.HttpContext.Request;
            if (filterContext.HttpContext.Session["UserId"] == null) // For Checking User is Logged in or Not 
            {
                objaudit.UserId = null;
                objaudit.LoginStatus = "Anomynous";
            }
            else
            {
                objaudit.UserId = filterContext.HttpContext.Session["UserId"].ToString();
                objaudit.LoginStatus = "Logged In";
            }
            objaudit.SessionId = HttpContext.Current.Session.SessionID; // Application SessionID
                                                                        // User IPAddress
            objaudit.IPAddress =
            request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            objaudit.PageAccessed = request.RawUrl;  // URL User Requested
            objaudit.LoggedInAt = DateTime.Now;      // Time User Logged In || And time User Request Method
            if (actionName.Equals("Logout"))
            {
                objaudit.LoggedInOut = DateTime.Now; // Time User Logged OUT
            }
            
            objaudit.ControllerName = controllerName; // ControllerName
            objaudit.ActionName = actionName;         // ActionName
            a_bal.AddAuditTrail(objaudit);
            base.OnActionExecuting(filterContext);
        }
    }
}