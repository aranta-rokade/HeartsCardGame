using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Hearts.DAL
{
    public class GameDAL
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Game GetGame(int gameId)
        {
            using (var db = new HeartsEntities())
            {
                var game = db.Games.Find(gameId);
                if (game == null)
                {
                    logger.Warn("GameId: {0} - not found.", gameId);
                    throw new CustomException("Invalid Game.");
                }
                return game;
            }
        }

        public int AddGame(int player1)
        {
            using (var db = new HeartsEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == player1);
                if (user == null)
                    throw new CustomException("Invalid user.");
                if (db.Games.FirstOrDefault(x => 
                        (
                           x.Player1 == player1 ||
                           x.Player2 == player1 ||
                           x.Player3 == player1 ||
                           x.Player4 == player1
                        )
                        && 
                        (
                            x.Status == (int)GameStatus.Started || x.Status == (int)GameStatus.Waiting)
                        ) != null)
                    throw new CustomException("User is already part of an another game. You can play only one game at a time.");

                Game game = new Game { Player1 = player1, Status = (int)GameStatus.Waiting, StartTime=DateTime.Now };
                
                db.Games.Add(game);
                db.SaveChanges();

                user.ActiveGameId = game.GameId;
                db.SaveChanges();

                return game.GameId;
            }
        }

        public Game AddPlayer(int playerid, int gameId)
        {
            using (var db = new HeartsEntities())
            {
                var g = db.Games.FirstOrDefault(x=>x.GameId == gameId);
                if (g.Player2 == null) g.Player2 = playerid;
                else if (g.Player3 == null) g.Player3 = playerid;
                else if (g.Player4 == null)
                {
                    g.Player4 = playerid;
                    g.Status = (int)GameStatus.Started;
                    g.StartTime = DateTime.Now;
                };
                if (g == null)
                {
                    logger.Warn("GameId: {0} - not found.", g.GameId);
                    throw new CustomException("Invalid Game.");
                }
                if (db.Users.FirstOrDefault(x => x.UserId == g.Player2 || x.UserId == g.Player3 || x.UserId == g.Player4) == null)
                    throw new CustomException("Invalid user.");
                db.SaveChanges();
                return g;
            }
        }

        public List<Game> GetAllWaitingGames() {
            using (var db = new HeartsEntities())
            {
                return db.Games
                    .Where(x=>x.Status == (int)GameStatus.Waiting)
                    .ToList();
            }
        }

        public Game GetActiveGame(int playerId)
        {
            using (var db = new HeartsEntities())
            {
                return db.Games
                    .FirstOrDefault(x => 
                        x.Player1 == playerId || 
                        x.Player2.GetValueOrDefault(0) == playerId ||
                        x.Player3.GetValueOrDefault(0) == playerId ||
                        x.Player4.GetValueOrDefault(0) == playerId);
            }
        }
    }
}
