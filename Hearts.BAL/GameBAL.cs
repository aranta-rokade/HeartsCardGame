using Hearts.DAL;
using System;

namespace Hearts.BAL
{
    public class GameBAL
    {
        public Game NewGame(int player1)
        {
            GameDAL gdal = new GameDAL();
            return gdal.AddGame(new Game
            { Player1 = player1, Status = 0, StartTime = DateTime.Now });
        }

        public Game JoinGame(int gameId, int playerid)
        {
            GameDAL gdal = new GameDAL();
            Game game = gdal.GetGame(gameId);
            
            if (game.Player2 != 0 && game.Player3 != 0 && game.Player4 != 0)
                throw new Exception("Game room full.");

            else if (game.Player2 == 0) game.Player2 = playerid;
            else if (game.Player3 == 0) game.Player3 = playerid;
            else if (game.Player4 == 0) game.Player4 = playerid;

            // update status is all palyers joined
            if (game.Player4 != 0)
                game.Status = 1;

            return gdal.UpdateGame(game);
        }
    }
}
