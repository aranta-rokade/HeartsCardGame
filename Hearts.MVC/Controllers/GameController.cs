using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hearts.MVC.Controllers
{
    [RequireHttps]
    [CustomAuthorize]
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            try
            {
                GameBAL gbal = new GameBAL();
                var games = gbal.GetAllWaitingGames();
                return View(games);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }

        // GET: Create
        public ActionResult Create()
        {
            try
            {
                GameBAL gbal = new GameBAL();
                var games = gbal.NewGame(Session["UserId"].ToString());
                TempData["IsSuccess"] = "success";
                TempData["Message"] = "New game created.";
                return View("Create", null, games);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("Index");
            }
        }

        // Get: Join/{id}
        public ActionResult Join(string hashedGameId)
        {
            try
            {
                GameBAL gbal = new GameBAL();
                var games = gbal.JoinGame(hashedGameId, Session["UserId"].ToString());
                TempData["IsSuccess"] = "success";
                TempData["Message"] = "You have joined the game.";
                return View("Create", null, games);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("Index");
            }
        }

        // Get: Join/{hashedGameId}
        public ActionResult Resume(string hashedGameId)
        {
            return View();
        }
    }
}