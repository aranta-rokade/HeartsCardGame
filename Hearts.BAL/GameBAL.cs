using Hearts.DAL;
using Hearts.ViewModel;
using System;
using Utility;

namespace Hearts.BAL
{
    public class GameBAL
    {
        public string NewGame(int player1)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                var gameId = gdal.AddGame(player1);
                //TODO: hash the game id and return game url
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

        public string JoinGame(int gameId, int playerid)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();

                Game game = gdal.GetGame(gameId);
                User user = udal.GetUserById(playerid);

                if (playerid == game.Player1 || playerid == game.Player2 || playerid == game.Player3 || playerid == game.Player4)
                {
                    //user is already part of the game
                    //TODO: hash the game id and return game url
                    return game.GameId.ToString();
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

                //TODO: hash the game id and return game url
                return new_game.GameId.ToString();

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
    }
}
