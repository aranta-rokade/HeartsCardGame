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
        public int PlayerId { get; set; }
        public int Points { get; set; }
        public string Name { get; private set; }

        public Card(Suit suit, Value value)
        {
            Value = value;
            Suit = suit;
            Points = CalculatePoints(this);
            Name = String.Format("{0} of {1}", value, suit);
        }

        public bool IsEqual(Card card)
        {
            return card.Value == Value && card.Suit == Suit;
        }

        public static int CalculatePoints(Card card)
        {
            if (card.Suit == Suit.Hearts)
                return 1;
            else if (card.Suit == Suit.Spades && card.Value == Value.Queen)
                return 13;
            else
                return 0;
        }
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

    

}
