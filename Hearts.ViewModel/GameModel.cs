using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.ViewModel
{
    public class GameModel
    {
        [Key]
        public int GameId { get; set; }

        [Display(Name = "Game URL")]
        public string GameURL { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Player 1")]
        public Player Player1 { get; set; }

        [Display(Name = "Player 2")]
        public Player Player2 { get; set; }

        [Display(Name = "Player 3")]
        public Player Player3 { get; set; }

        [Display(Name = "Player 4")]
        public Player Player4 { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }

        List<Card> Deck;

        public void InitializeDeck()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Value value in Enum.GetValues(typeof(Value))) {
                    Card cardToAdd = new Card(suit, value);
                    Deck.Add(cardToAdd);
                }
        }

        public void Shuffle()
        {
            List<Card> temporaryDeck = new List<Card>();
            Random random = new Random();
            while (Deck.Count > 0)
            {
                int index = random.Next(0, Deck.Count);
                Deck.RemoveAt(index);
                temporaryDeck.Add(Deck[index]);
            }
            Deck = temporaryDeck;
        }

        private Card Deal() 
        {
            Random random = new Random();
            int index = random.Next(0, Deck.Count);
            Card card = Deck[index];
            Deck.RemoveAt(index);
            return card;
        }

        public void DealCards()
        {
            for (int i = 0; i < 13; i++)
            {
                Player1.Hand.Add(Deal());
                Player2.Hand.Add(Deal());
                Player3.Hand.Add(Deal());
                Player4.Hand.Add(Deal());
            }
        }
    }
}
