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
                var gameId = hashing.Encrypt(gdal.AddGame(playerId).ToString());
                return gameId.ToString();
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

                else if (game.Player2 == null) game.Player2 = playerid;
                else if (game.Player3 == null) game.Player3 = playerid;
                else if (game.Player4 == null) {
                    game.Player4 = playerid;
                    game.Status = (int)GameStatus.Started;
                    game.StartTime = DateTime.Now;
                }

                var new_game = gdal.AddPlayer(playerid, game.GameId);
                udal.UpdateActiveGame(playerid, new_game.GameId);

                return hashing.Encrypt(new_game.GameId.ToString());

                //return new GameModel
                //{
                //    GameId = new_game.GameId,
                //    Player1 = new_game.Player1,
                //    Player2 = new_game.Player2,
                //    Player3 = new_game.Player3,
                //    Player4 = new_game.Player4,
                //    Status = new_game.Status,
                //    StartTime = new_game.StartTime
                //};
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
                        GameId = game.GameId,
                        GameURL = hashing.Encrypt(game.GameId.ToString()),
                        Status = game.Status,
                        EndTime = game.EndTime,
                        StartTime = game.StartTime
                    };
                    var player = udal.GetUserById(game.Player1);
                    new_game.Player1 = new Player(player.UserId, player.Username);
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

        //public GameModel GetGame(int)
    }
}
