using Hearts.BAL;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
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
                    UserModel user = u_bal.GetUser(model.UserName);
                    Session["UserId"] = user.UserId.ToString();
                    Session["UserName"] = user.UserName.ToString();

                    //var ident = new ClaimsIdentity(
                    //  new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    //new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    //new Claim(ClaimTypes.Name,user.UserName),
                    //        },
                    //    "Password");

                    //ClaimsPrincipal principal = new ClaimsPrincipal(ident);
                    //Thread.CurrentPrincipal = principal;

                    //HttpContext.User .Authentication.SignIn(
                    //   new AuthenticationProperties { IsPersistent = false }, ident);

                    //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = persistCookie }, identity);

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
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
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
                var new_user = u_bal.AddUser(model.UserName, model.EmailId, model.Password);
                if (new_user != null)
                {
                    UserModel user = u_bal.GetUser(model.UserName);
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
        public ActionResult Index()
        {
            try
            {
                UserBAL u_bal = new UserBAL();
                UserModel user = u_bal.GetUser(Convert.ToInt16((Session["UserId"].ToString())));
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
        public ActionResult Logout()
        {
            try
            {
                Session["UserId"] = "";
                Session["UserName"] = "";
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