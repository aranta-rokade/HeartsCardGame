using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class Card
    {
        public Suit Suit { get; set; }
        public Value Value { get; set; }
        public int playerId { get; set; }

    }

    public enum Suit {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }
    public enum Value
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Move {
        public Card card1 { get; set; }
        public Card card2 { get; set; }
        public Card card3 { get; set; }
        public Card card4 { get; set; }

        //public Card getHighest() {
            //if(card1.Value > card2.Value)
            //    winner1 = card1
            //else 
            //    winner1 = card2

            //if(card3.Value > card4.Value)
            //    winner2 = card3
            //else
            //    winner1 = card2
        //}
    }

}
