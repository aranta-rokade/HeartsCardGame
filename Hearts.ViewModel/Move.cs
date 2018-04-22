using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class Move
    {
        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }

        public Suit LeadingSuit { get; set; }

        //public card getHighest()
        //{
        //    if (card1.Suit == card2.suit)
        //    {
        //    }
        //    if (card1.value > card2.value)
        //        winner1 = card1
        //    else
        //        winner1 = card2

        //    if (card3.value > card4.value)
        //        winner2 = card3
        //    else
        //        winner1 = card2
        //}


    }
}
