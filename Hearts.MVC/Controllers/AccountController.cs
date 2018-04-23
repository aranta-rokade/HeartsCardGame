using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hearts.MVC.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            Random random = new Random();
            #pragma warning disable 618
            var seed = FormsAuthentication.HashPasswordForStoringInConfigFile(
                    random.Next().ToString(), "MD5");
            #pragma warning restore 618
            LoginModel model = new LoginModel
            {
                hashRandomSeed = seed
            };
            return View(model);
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
                    Session["UserName"] = user.UserName.ToString();

                    // Getting New Guid
                    string guid = Convert.ToString(Guid.NewGuid());
                    //Storing new Guid in Session
                    Session["AuthenticationToken"] = guid;
                    //Adding Cookie in Browser
                    Response.Cookies.Add(new HttpCookie("AuthenticationToken", guid));

                    return RedirectToAction("Index", "Home"); // auth succeed 

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
            Random random = new Random();
            #pragma warning disable 618
            var seed = FormsAuthentication.HashPasswordForStoringInConfigFile(
                    random.Next().ToString(), "MD5");
            #pragma warning restore 618
            RegisterModel model = new RegisterModel
            {
                hashRandomSeed = seed
            };
            return View(model);
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