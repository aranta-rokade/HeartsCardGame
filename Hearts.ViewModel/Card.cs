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
        public bool IsSlected { get; set; }
        public string Name {
            get
            {
                return String.Format("{0} of {1}", Value, Suit);
            }
        }
        public string Id
        {
            get
            {
                return String.Format("{0}-{1}", Value, Suit);
            }
        }

        public Card(Suit suit, Value value)
        {
            Value = value;
            Suit = suit;
            Points = CalculatePoints(this);
        }

        public Card()
        {
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

        public void ConvertStringToCard(string card)
        {
            string[] x = card.Split('-');
            this.Value = (Value)Enum.Parse(typeof(Value), x[0]);
            this.Suit = (Suit)Enum.Parse(typeof(Suit), x[1]);
            this.Points = CalculatePoints(this);
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
