using Hearts.DAL;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using Utility;

namespace Hearts.BAL
{
    public class GameBAL
    {
        public string NewGame(string hashedPlayerId)
        {
            try
            {
                Hashing hashing = new Hashing();
                int playerId = Convert.ToInt32(hashing.Decrypt(hashedPlayerId));
                GameDAL gdal = new GameDAL();
                int gameId = gdal.AddGame(playerId);
                var hashed_gameId = hashing.Encrypt(gameId.ToString());
                // Wait for other players to join
                WaitForPlayers(90, gameId);
                var game = gdal.GetGame(gameId);
                // check if game room is full
                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                {
                    return hashed_gameId.ToString();
                }
                AbortGame(hashed_gameId);
                throw new CustomException("Timeout. Players insufficient.");
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public string JoinGame(string hashedGameId, string hashedPlayerid)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                Hashing hashing = new Hashing();

                int gameId = Convert.ToInt32(hashing.Decrypt(hashedGameId));
                int playerid = Convert.ToInt32(hashing.Decrypt(hashedPlayerid));

                Game game = gdal.GetGame(gameId);
                User user = udal.GetUserById(playerid);

                if (playerid == game.Player1 || playerid == game.Player2 || playerid == game.Player3 || playerid == game.Player4)
                {
                    //user is already part of the game
                    return hashing.Encrypt(game.GameId.ToString());
                }

                //user is already part of ANOTHER the game
                if (user.ActiveGameId != null)
                    throw new CustomException("You are already a part of another game.");

                if (game.Player2 != null && game.Player3 != null && game.Player4 != null)
                    throw new CustomException("Game room is full.");

                int time = 0;
                if (game.Player1 == null)
                {
                    game.Player1 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }
                else if (game.Player2 == null)
                {
                    game.Player2 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }
                else if (game.Player3 == null)
                {
                    game.Player3 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }
                else if (game.Player4 == null)
                {
                    game.Player4 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }

                var new_game = gdal.AddPlayer(playerid, game.GameId);
                udal.UpdateActiveGame(playerid, new_game.GameId);

                WaitForPlayers(time, new_game.GameId);

                game = gdal.GetGame(game.GameId);

                // check if game room is full
                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                {
                    return hashing.Encrypt(new_game.GameId.ToString());
                }

                AbortGame(hashedGameId);
                throw new CustomException("Timeout. Players insufficient.");
                
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public GameModel GetGame(string hashedGameId)
        {
            GameDAL gdal = new GameDAL();
            UserDAL udal = new UserDAL();
            Hashing unhash = new Hashing();
            int gameId = Convert.ToInt32(unhash.Decrypt(hashedGameId));
            var game = gdal.GetGame(gameId);
            GameModel gameModel = new GameModel
            {
                //GameId = game.GameId,
                GameURL = unhash.Encrypt(game.GameId.ToString()),
                Status = game.Status,
                EndTime = game.EndTime,
                StartTime = game.StartTime
            };
            User player;
            if (game.Player1 != null)
            {
                player = udal.GetUserById(game.Player1.Value);
                gameModel.Player1 = new Player(player.UserId, player.Username);
            }
            if (game.Player2 != null)
            {
                player = udal.GetUserById(game.Player2.Value);
                gameModel.Player2 = new Player(player.UserId, player.Username);
            }
            if (game.Player3 != null)
            {
                player = udal.GetUserById(game.Player3.Value);
                gameModel.Player3 = new Player(player.UserId, player.Username);
            }
            if (game.Player4 != null)
            {
                player = udal.GetUserById(game.Player4.Value);
                gameModel.Player4 = new Player(player.UserId, player.Username);
            }
            return gameModel;
        }

        public List<GameModel> GetAllWaitingGames()
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                Hashing hashing = new Hashing();

                var games = gdal.GetAllWaitingGames();
                List<GameModel> waiting_game = new List<GameModel>();

                foreach (var game in games)
                {
                    var new_game = new GameModel
                    {
                        //GameId = game.GameId,
                        GameURL = hashing.Encrypt(game.GameId.ToString()),
                        Status = game.Status,
                        EndTime = game.EndTime,
                        StartTime = game.StartTime
                    };
                    User player;
                    if (game.Player1 != null)
                    {
                        player = udal.GetUserById(game.Player1.Value);
                        new_game.Player1 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player2 != null) {
                        player = udal.GetUserById(game.Player2.Value);
                        new_game.Player2 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player3 != null)
                    {
                        player = udal.GetUserById(game.Player3.Value);
                        new_game.Player3 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player4 != null)
                    {
                        player = udal.GetUserById(game.Player4.Value);
                        new_game.Player4 = new Player(player.UserId, player.Username);
                    }
                    waiting_game.Add(new_game);
                }
                return waiting_game;
                
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }

        }

        public void WaitForPlayers(int seconds, int gameId)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }

        //public GameModel GetGame(int)
        public bool AbortGame(string hashedgameId)
        {
            GameDAL gdal = new GameDAL();
            Hashing unhash = new Hashing();
            int gameId =Convert.ToInt32( unhash.Decrypt(hashedgameId));
            return gdal.AbortGame(gameId);
            
        }
    }
}
