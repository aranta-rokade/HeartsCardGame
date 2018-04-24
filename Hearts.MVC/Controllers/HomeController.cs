using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC.Controllers
{
    [RequireHttps]
    [NoCache]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Application Decription.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Like our website? Some information about us. ";

            return View();
        }
        public ActionResult Rules()
        {
            ViewBag.Message = "The rules of the game.";

            return View();
        }
        public ActionResult LeaderBoard()
        {
            try
            {
                ViewBag.Message = "Hall of Fame.";
                UserBAL u_bal = new UserBAL();
                List<UserModel> list = u_bal.GetAllUsers();
                return View(list);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("Index");
            }
            
        }

    }
}