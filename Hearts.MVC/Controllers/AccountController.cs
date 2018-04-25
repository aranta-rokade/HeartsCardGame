using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using Hearts.ViewModel;
using System;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC.Controllers
{
    [RequireHttps]
    [NoCache]
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                UserBAL u_bal = new UserBAL();
                if (u_bal.ValidateUser(model.UserName, model.Password))
                {
                    UserModel user = u_bal.GetUserByUserName(model.UserName);
                    Session["UserId"] = user.UserId;
                    Session["UserName"] = user.UserName;
                    Session["ActiveGameId"] = user.ActiveGameId;

                    // Getting New Guid
                    string guid = Convert.ToString(Guid.NewGuid());
                    //Storing new Guid in Session
                    Session["AuthenticationToken"] = guid;
                    //Adding Cookie in Browser
                    Response.Cookies.Add(new HttpCookie("AuthenticationToken", guid));

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home"); 

                }
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = "Username or password incorrect.";
                ModelState.AddModelError("", "Invalid Credentials");
                return View();
            }
            catch (Exception e)
            {
                // invalid username or password
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return View();
            }
            
            
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                UserBAL u_bal = new UserBAL();
                var result = u_bal.AddUser(model.UserName, model.EmailId, model.Password);
                if (result)
                {
                    TempData["IsSuccess"] = "success"; 
                    TempData["Message"] = "Registeration successfull.";
                    return RedirectToAction("Login"); // auth succeed 
                }
                // invalid username or password
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = "Registeration unsuccessfull.";
                ModelState.AddModelError("", "Registeration unsuccessfull.");
                return View();
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return View();
            }
            
        }

        // GET: /Account
        [CustomAuthorize]
        public ActionResult Index()
        {
            try
            {
                UserBAL u_bal = new UserBAL();
                UserModel user = u_bal.GetUserById(Session["UserId"].ToString());
                if (user == null)
                {
                    TempData["IsSuccess"] = "danger";
                    TempData["Message"] = "User not logged in.";
                }
                return View(user);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }

        // GET: /Account/Logout
        [CustomAuthorize]
        public ActionResult Logout()
        {
            try
            {
                //Removing Session
                Session.Abandon();
                Session.Clear();
                Session.RemoveAll();

                //Removing _RequestVerificationToken Cookie
                if (Request.Cookies["_RequestVerificationToken"] != null)
                {
                    Response.Cookies["_RequestVerificationToken"].Value = string.Empty;
                    Response.Cookies["_RequestVerificationToken"].Expires = DateTime.Now.AddMonths(-10);
                }
                //Removing ASP.NET_SessionId Cookie
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
                }
                //Removing AuthenticationToken Cookie
                if (Request.Cookies["AuthenticationToken"] != null)
                {
                    Response.Cookies["AuthenticationToken"].Value = string.Empty;
                    Response.Cookies["AuthenticationToken"].Expires = DateTime.Now.AddMonths(-10);
                }

                TempData["IsSuccess"] = "success";
                TempData["Message"] = "You have successfully logged out of the system.";
                return RedirectToAction("Index","Home");
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }
    }
}