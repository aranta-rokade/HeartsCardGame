using Hearts.BAL;
using Hearts.MVC.CustomAttributes;
using Hearts.ViewModel;
using Microsoft.AspNet.SignalR;
using System;
using System.Web.Mvc;
using Hearts.MVC.PublisherSubcriber;

namespace Hearts.MVC.Controllers
{
    [RequireHttps]
    [CustomAuthorize]
    [NoCache]
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

        // Get: Join/{hashedGameId}
        public ActionResult Join(string hashedGameId)
        {
            try
            {
                GameBAL gbal = new GameBAL();
                var gameId = gbal.JoinGame(hashedGameId, Session["UserId"].ToString());
                TempData["IsSuccess"] = "success";
                TempData["Message"] = "You have joined the game.";

                var hub = GlobalHost.ConnectionManager.GetHubContext<GameRoomHub>();
                hub.Groups.Add(Session["UserId"].ToString(), gameId);
                hub.Clients.Group(gameId).addNewMessageToPage(Session["UserName"].ToString() + " joined the game.");
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

        // Get: Game/{hashedGameId}
        public ActionResult Game(string hashedGameId)
        {
            try
            {
                GameBAL gbal = new GameBAL();
                var game = gbal.GetGame(hashedGameId, Session["UserId"].ToString());

                game.CardsToPass = null;
                if (game.Player1.UserId.Equals(Session["UserId"].ToString()))
                    game.CardsToPass = game.Player1.Hand;
                if (game.Player2.UserId.Equals(Session["UserId"].ToString()))
                    game.CardsToPass = game.Player2.Hand;
                if (game.Player3.UserId.Equals(Session["UserId"].ToString()))
                    game.CardsToPass = game.Player3.Hand;
                if (game.Player4.UserId.Equals(Session["UserId"].ToString()))
                    game.CardsToPass = game.Player4.Hand;
                return View(game);
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MakeMove(GameModel game)
        {
            GameBAL g_bal = new GameBAL();
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(game);
                }
                string message="";
                string notification = "hello";
                var gamebal = g_bal.GetGame(game.GameURL, Session["UserId"].ToString());

                if (gamebal.PassOrPlay == 1)
                {
                    message = "Your cards are to pass are chosen. Wait for other to choose thier cards.";
                    notification = " is ready to pass cards.";
                }
                else
                {
                    message = "Your card is played.";
                    notification = "played card : " + game.CardSelectedString;
                }

                var hub = GlobalHost.ConnectionManager.GetHubContext<GameRoomHub>();

                var newGame = g_bal.MakeMove(game, Session["UserId"].ToString());
                if (newGame!=null)
                {
                    TempData["IsSuccess"] = "success";
                    TempData["Message"] = message;
                    hub.Clients.All.addNewMessageToPage(Session["UserName"].ToString(), notification);
                    hub.Clients.All.updateNewPlayer(newGame.CurrentTurn);
                }
                else
                {
                    TempData["IsSuccess"] = "danger";
                    TempData["Message"] = "Your cards are not passed.";
                }
                return RedirectToAction("Game", "Game", new { hashedGameId = game.GameURL });
            }
            catch (Exception e)
            {
                TempData["IsSuccess"] = "danger";
                TempData["Message"] = e.Message;
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("Game", "Game", new { hashedGameId = game.GameURL });
            }
        }
    }
}