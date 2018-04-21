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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserBAL u_bal = new UserBAL();
            if (u_bal.ValidateUser(model.UserName, model.Password))
            {
                UserModel user = u_bal.GetUser(model.UserName);
                var ident = new ClaimsIdentity(
                  new[] { 
                  // adding following 2 claim just for supporting default antiforgery provider
                  new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                  new Claim(ClaimTypes.Name,user.UserName),
                      },
                  "Password");

                ClaimsPrincipal principal = new ClaimsPrincipal(ident);
                //Thread.CurrentPrincipal = principal;

                //HttpContext.User .Authentication.SignIn(
                //   new AuthenticationProperties { IsPersistent = false }, ident);

                //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = persistCookie }, identity);

                return RedirectToAction("MyAction"); // auth succeed 
            }
            // invalid username or password
            ModelState.AddModelError("", "invalid username or password");
            return View();
        }


    }
}