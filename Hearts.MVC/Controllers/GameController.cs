using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using System;
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
                var gameId = gbal.NewGame(Session["UserId"].ToString());
                TempData["IsSuccess"] = "success";
                TempData["Message"] = "New game created.";
                if (gameId != null)
                    return RedirectToAction("Game", "Game", new { hashedGameId = gameId });
                else
                {
                    TempData["IsSuccess"] = "danger";
                    TempData["Message"] = "Game Not Created";
                    return RedirectToAction("Index");

                }
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
                var gameId = gbal.JoinGame(hashedGameId, Session["UserId"].ToString());
                TempData["IsSuccess"] = "success";
                TempData["Message"] = "You have joined the game.";
                return RedirectToAction("Game", "Game", new { hashedGameId=gameId });
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
        public ActionResult Game(string hashedGameId)
        {
            GameBAL gbal = new GameBAL();
            var game = gbal.GetGame(hashedGameId);
            return View(game);
        }
    }
}