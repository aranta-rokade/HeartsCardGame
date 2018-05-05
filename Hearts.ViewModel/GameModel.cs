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
        
        List<Card> Deck { get; set; }

        [Display(Name = "Player")]
        public int PlayerTurn { get; set; }

        [Display(Name = "Turn")]
        public string CurrentTurn { get; set; }

        [Display(Name = "Pass or Play")]
        public int PassOrPlay { get; set; }

        [Display(Name = "Selected Card")]
        public Card CardSelected { get; set; }

        public Boolean HeartsPlayed { get; set; }
        public Boolean SpadesPlayed { get; set; }

        public string CardSelectedString { get; set; }

        public List<Card> CardsToPass { get; set; }

        [Display(Name = "Round")]
        public int GameRound { get; set; }

        public MoveModel Trick { get; set; }
        [Display(Name = "Winner")]
        public string Winner { get; set; }

        public void InitializeDeck()
        {
            Deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Value value in Enum.GetValues(typeof(Value))) {
                    Card cardToAdd = new Card(suit, value);
                    Deck.Add(cardToAdd);
                }
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
