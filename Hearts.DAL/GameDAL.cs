using NLog;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

                Game game = new Game { Player1 = player1, Status = (int)GameStatus.Waiting, StartTime = DateTime.Now, Player1StartTime = DateTime.Now };
                
                db.Games.Add(game);
                db.SaveChanges();

                user.ActiveGameId = game.GameId;
                logger.Info("GamedId: {0} - added.", game.GameId);
                db.SaveChanges();

                return game.GameId;
            }
        }

        public Game AddPlayer(int playerid, int gameId)
        {
            using (var db = new HeartsEntities())
            {
                var g = db.Games.FirstOrDefault(x=>x.GameId == gameId);
                if (g.Status==(int)GameStatus.Aborted || g.Status == (int)GameStatus.Ended)
                {
                    logger.Warn("GameId: {0} - doesn't exist anymore..", gameId);
                    throw new CustomException("Game doesn't exist anymore.");
                }
                if (g.Player1 == null)
                {
                    g.Player1 = playerid;
                    g.Player1StartTime = DateTime.Now;
                }
                else if (g.Player2 == null)
                {
                    g.Player2 = playerid;
                    g.Player2StartTime = DateTime.Now;
                }
                else if (g.Player3 == null)
                {
                    g.Player3 = playerid;
                    g.Player3StartTime = DateTime.Now;
                }
                else if (g.Player4 == null)
                {
                    g.Player4 = playerid;
                    g.Player4StartTime = DateTime.Now;
                    g.StartTime = DateTime.Now;
                };

                if(g.Player1!=null && g.Player2 != null && g.Player3 != null && g.Player4 != null)
                    g.Status = (int)GameStatus.Started;

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

        public bool AbortGame(int gameId)
        {
            using (var db = new HeartsEntities())
            {
                var game = db.Games.FirstOrDefault(x=> x.GameId==gameId);
                if(game.Status == (int)GameStatus.Ended)
                    throw new CustomException("Game has ended.");

                if (game == null)
                {
                    logger.Warn("GameId: {0} - not found.", game.GameId);
                    throw new CustomException("Invalid game.");
                }

                var playerids = db.Users.Where(x => x.ActiveGameId == gameId);
                foreach (var player in playerids)
                    player.ActiveGameId = null;

                game.Status = (int)GameStatus.Aborted;
                logger.Info("GameId: {0} - aborted.", game.GameId);
                db.SaveChanges();
                return true;
           
            }
        }

        public void UpdateGameAfterMove(Game update_game, int userId)
        {
            using (var db = new HeartsEntities())
            {
                var game = db.Games.FirstOrDefault(x=>x.GameId == update_game.GameId);
                if (game == null)
                    throw new CustomException("Invalid Game.");

                game.Turn = update_game.Turn;
                game.LeadingSuit = update_game.LeadingSuit;

                game.Player1Hand = update_game.Player1Hand;
                game.Player2Hand = update_game.Player2Hand;
                game.Player3Hand = update_game.Player3Hand;
                game.Player4Hand = update_game.Player4Hand;

                if (game.PassOrPlay == 2)
                {
                    game.Player1Stash = update_game.Player1Stash;
                    game.Player2Stash = update_game.Player2Stash;
                    game.Player3Stash = update_game.Player3Stash;
                    game.Player4Stash = update_game.Player4Stash;

                    game.Player1Score = update_game.Player1Score;
                    game.Player2Score = update_game.Player2Score;
                    game.Player3Score = update_game.Player3Score;
                    game.Player4Score = update_game.Player4Score;

                    game.Player1Card = update_game.Player1Card;
                    game.Player2Card = update_game.Player2Card;
                    game.Player3Card = update_game.Player3Card;
                    game.Player4Card = update_game.Player4Card;

                }

                else if (game.PassOrPlay == 1)
                {
                    if (userId == game.Player1) game.Player1Trash = update_game.Player1Trash;
                    if (userId == game.Player2) game.Player2Trash = update_game.Player2Trash;
                    if (userId == game.Player3) game.Player3Trash = update_game.Player3Trash;
                    if (userId == game.Player4) game.Player4Trash = update_game.Player4Trash;

                    if(game.Player1Trash != null && game.Player2Trash !=null && game.Player3Trash != null && game.Player4Trash != null)
                    {
                        if (game.Player1Trash.Length > 1 && game.Player2Trash.Length > 1 && game.Player3Trash.Length > 1 && game.Player4Trash.Length > 1)
                        {
                            // Passing the trash
                            if (game.GameRound.Value == 1)
                            {
                                game.Player1Hand += game.Player4Trash;
                                game.Player2Hand += game.Player1Trash;
                                game.Player3Hand += game.Player2Trash;
                                game.Player4Hand += game.Player3Trash;
                            }
                            else if (game.GameRound.Value == 2)
                            {
                                game.Player4Hand += game.Player1Trash;
                                game.Player3Hand += game.Player4Trash;
                                game.Player2Hand += game.Player3Trash;
                                game.Player1Hand += game.Player2Trash;
                            }
                            else if (game.GameRound.Value == 3)
                            {
                                game.Player1Hand += game.Player3Trash;
                                game.Player3Hand += game.Player1Trash;
                                game.Player2Hand += game.Player4Trash;
                                game.Player4Hand += game.Player2Trash;
                            }
                            game.Player1Trash = null;
                            game.Player2Trash = null;
                            game.Player3Trash = null;
                            game.Player4Trash = null;

                            game.PassOrPlay = 2;
                        }
                    }
                }
               
                db.SaveChanges();
            }
        }

        public void UpdateGameAfterInit(Game initGame)
        {
            using (var db = new HeartsEntities())
            {
                var game = db.Games.FirstOrDefault(x=>x.GameId == initGame.GameId);
                if (game == null)
                {
                    logger.Info("GameId: {0} - invalid game.", game.GameId);
                    throw new CustomException("Invalid Game.");
                }
                game.Turn = initGame.Turn;
                game.Status = initGame.Status;
                game.PassOrPlay = initGame.PassOrPlay;
                game.GameRound = initGame.GameRound;

                game.Player1Hand = initGame.Player1Hand;
                game.Player2Hand = initGame.Player2Hand;
                game.Player3Hand = initGame.Player3Hand;
                game.Player4Hand = initGame.Player4Hand;

                db.SaveChanges();
            }
        }
    }
}
