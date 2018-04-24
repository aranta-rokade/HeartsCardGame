using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class Player
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool PassOrSelect { get; set; }

        public List<Card> Hand { get; set; }

        public List<Card> CardsToPass { get; set; }

        public List<Card> Garbage { get; set; }

        private Card currentCard;
        public Card CurrentCard
        {
            get { return currentCard; }
            set
            {
                //TODO: check if setter removes played card
                currentCard = value;
                Hand.Remove(value);
            }
        }

        private int points;
        public int Points
        {
            get
            {
                if (Garbage != null && Garbage.Count > 0)
                {
                    points = 0;

                    foreach (Card card in Garbage)
                    {
                        points += card.Points;
                    }
                }

                return points;
            }
        }

        public Player(int userId, string username)
        {
            Garbage = new List<Card>();
            PassOrSelect = true;
            Hand = new List<Card>();
            CardsToPass = new List<Card>();
            //UserId = userId;
            UserName = username;
        }

        public Card SelectCard(int index)
        {
            return Hand[index];
        }
    }
}
