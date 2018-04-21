using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class GameModel
    {
        public int GameId { get; set; }

        public int Status { get; set; }

        public int Player1 { get; set; }

        public Nullable<int> Player2 { get; set; }

        public Nullable<int> Player3 { get; set; }

        public Nullable<int> Player4 { get; set; }

        public int Player1Score { get; set; }

        public int Player2Score { get; set; }

        public int Player3Score { get; set; }

        public int Player4Score { get; set; }

        public System.DateTime StartTime { get; set; }

        public Nullable<System.DateTime> EndTime { get; set; }
    }
}
