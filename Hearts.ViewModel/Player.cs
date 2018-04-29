using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Hearts.ViewModel
{
    public class Player
    {
        public string UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public bool PassOrSelect { get; set; }

        [Display(Name = "Cards in Hand")]
        public List<Card> Hand { get; set; }

        [Display(Name = "Cards to Pass")]
        public List<Card> CardsToPass { get; set; }

        public List<Card> Stash { get; set; }

        [Display(Name = "Card Chosen")]

        public Card CurrentCard{get;set;}

        private int points;
        [Display(Name = "Score")]
        public int Points
        {
            get
            {
                if (Stash != null && Stash.Count > 0)
                {
                    points = 0;

                    foreach (Card card in Stash)
                    {
                        points += card.Points;
                    }
                }

                return points;
            }
        }

        public Player(int userId, string username)
        {
            Stash = new List<Card>();
            PassOrSelect = true;
            Hand = new List<Card>();
            CardsToPass = new List<Card>();
            UserId = new CustomCrypto().Encrypt(Convert.ToString(userId));
            UserName = username;
        } 

        public Player()
        {
            Stash = new List<Card>();
            PassOrSelect = true;
            Hand = new List<Card>();
            CardsToPass = new List<Card>();
        }

        public Card SelectCard(int index)
        {
            var card = Hand[index];
            Hand.Remove(card);
            return card;
        }
    }
}
