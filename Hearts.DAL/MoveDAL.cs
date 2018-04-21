using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.DAL
{
    public class MoveDAL
    {
        public Move AddMove(Move move)
        {
            using (var db = new HeartsEntities())
            {
                if (db.Games.FirstOrDefault(x => x.GameId == move.GameId) == null)
                    throw new Exception("Game not found.");
                if (db.Users.FirstOrDefault(x => x.UserId == move.PlayerId) == null)
                    throw new Exception("User not found.");
                if(db.Games.FirstOrDefault(
                    x => x.GameId == move.GameId && (
                          x.Player1!=move.PlayerId || x.Player2 != move.PlayerId ||
                          x.Player3 != move.PlayerId || x.Player4 != move.PlayerId)) == null)
                    throw new Exception("User not in game.");
                db.Moves.Add(move);
                db.SaveChanges();
                return move;
            }
        }
    }
}
