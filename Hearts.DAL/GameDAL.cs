using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    logger.Warn("GameId: {0} - Logged in.", gameId);
                    throw new Exception("Game not found.");
                }
                return game;
            }
        }
        public Game AddGame(Game game)
        {
            using (var db = new HeartsEntities())
            {
                db.Games.Add(game);
                db.SaveChanges();
                return game;
            }
        }

        public Game UpdateGame(Game game)
        {
            using (var db = new HeartsEntities())
            {
                var g = db.Games.FirstOrDefault(x=>x.GameId == game.GameId);
                if (g == null)
                {
                    logger.Warn("GameId: {0} - Logged in.", game.GameId);
                    throw new Exception("Game not found.");
                }
                db.Games.Add(game);
                db.SaveChanges();
                return game;
            }
        }
    }
}
