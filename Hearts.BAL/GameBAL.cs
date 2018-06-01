using Hearts.DAL;
using Hearts.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Hearts.BAL
{
    public class GameBAL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string NewGame(string hashedPlayerId)
        {
            try
            {
                CustomCrypto hashing = new CustomCrypto();
                int playerId = Convert.ToInt32(hashing.Decrypt(hashedPlayerId));
                GameDAL gdal = new GameDAL();
                int gameId = gdal.AddGame(playerId);
                var hashed_gameId = hashing.Encrypt(gameId.ToString());
                // Wait for other players to join
                WaitForPlayers(30, gameId);
                var game = gdal.GetGame(gameId);
                // check if game room is full
                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                {
                    return hashed_gameId.ToString();
                }
                AbortGame(hashed_gameId);
                throw new CustomException("Timeout. Players insufficient.");
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public string JoinGame(string hashedGameId, string hashedPlayerid)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                CustomCrypto hashing = new CustomCrypto();

                int gameId = Convert.ToInt32(hashing.Decrypt(hashedGameId));
                int playerid = Convert.ToInt32(hashing.Decrypt(hashedPlayerid));

                Game game = gdal.GetGame(gameId);
                User user = udal.GetUserById(playerid);

                if (playerid == game.Player1 || playerid == game.Player2 || playerid == game.Player3 || playerid == game.Player4)
                {
                    //user is already part of the game
                    return hashing.Encrypt(game.GameId.ToString());
                }

                //user is already part of ANOTHER the game
                if (user.ActiveGameId != null)
                    throw new CustomException("You are already a part of another game.");

                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                    throw new CustomException("Game room is full.");

                int time = 0;
                if (game.Player1 == null)
                {
                    game.Player1 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 30 - secondsElapsed;
                }
                else if (game.Player2 == null)
                {
                    game.Player2 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 30 - secondsElapsed;
                }
                else if (game.Player3 == null)
                {
                    game.Player3 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 30 - secondsElapsed;
                }
                else if (game.Player4 == null)
                {
                    game.Player4 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 30 - secondsElapsed;
                }

                var new_game = gdal.AddPlayer(playerid, game.GameId);
                udal.UpdateActiveGame(playerid, new_game.GameId);


                // check if game room is full
                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                {
                    InitializeGame(game.GameId);
                    WaitForPlayers(time, new_game.GameId);
                    return hashing.Encrypt(new_game.GameId.ToString());
                }

                WaitForPlayers(time, new_game.GameId);
                
                game = gdal.GetGame(game.GameId);
                if (game.Status != (int)GameStatus.Started)
                {
                    AbortGame(hashedGameId);
                    throw new CustomException("Timeout. Players insufficient.");
                }
                else return hashing.Encrypt(new_game.GameId.ToString());
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public GameModel MakeMove(GameModel gameModel, string hashedUserId)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                CustomCrypto unhash = new CustomCrypto();

                var g = unhash.Decrypt(gameModel.GameURL);
                int gameId = Convert.ToInt32(g);
                int userId = Convert.ToInt32(unhash.Decrypt(hashedUserId));

                var game = gdal.GetGame(gameId);
                var user = udal.GetUserById(userId);

                if (user.ActiveGameId != game.GameId)
                    throw new CustomException("You are not a part of this game.");

                //set cards in hand
                List<Card> cardsInHand = null;
                if (game.Player1.Value == userId)
                    cardsInHand = ConvertStringtoListCard(game.Player1Hand, game.Player1.Value);
                else if (game.Player2.Value == userId)
                    cardsInHand = ConvertStringtoListCard(game.Player2Hand, game.Player2.Value);
                else if (game.Player3.Value == userId)
                    cardsInHand = ConvertStringtoListCard(game.Player3Hand, game.Player3.Value);
                else if (game.Player4.Value == userId)
                    cardsInHand = ConvertStringtoListCard(game.Player4Hand, game.Player4.Value);

                string stash = "";

                // check pass the trash
                if (game.PassOrPlay == 1)
                {
                    if ((game.Player1 == userId && game.Player1Trash!=null && game.Player1Trash.Length > 1)
                        || (game.Player2 == userId && game.Player2Trash != null && game.Player2Trash.Length > 1)
                        || (game.Player3 == userId && game.Player3Trash != null && game.Player3Trash.Length > 1)
                        || (game.Player4 == userId && game.Player4Trash != null && game.Player4Trash.Length > 1))
                        throw new CustomException("You have already passed the trash. Wait for the game to begin.");


                    if (gameModel.CardsToPass.FindAll(x => x.IsSlected).Count != 3)
                        throw new CustomException("Select only THREE cards to pass.");
                    if (gameModel.CardsToPass.FirstOrDefault(x => x.IsSlected && x.Suit == Suit.Clubs && x.Value == Value.Two)!=null)
                        throw new CustomException("You can not pass Two of Clubs.");
                    //check valid card
                    foreach (var item in gameModel.CardsToPass)
                    {
                        if (item.IsSlected)
                        {
                            if(item.Suit == Suit.Clubs && item.Value == Value.Two)
                                throw new CustomException(String.Format("{0} You cannot pass this : ", item.Name));
                            if (cardsInHand.Find(x => x.Suit == item.Suit && x.Value == item.Value) == null)
                                throw new CustomException(String.Format("{0} Invalid card. You do not hve this card in your hand", item.Name));
                            else
                                cardsInHand.RemoveAll(x => x.Suit == item.Suit && x.Value == item.Value);
                        }
                    }
                }

                //check card to play
                else if (game.PassOrPlay == 2)
                {
                    if (game.Turn != userId)
                        throw new CustomException("It it not your turn.");
                    if (gameModel.CardSelectedString == null || gameModel.CardSelectedString.Length < 3)
                        throw new CustomException("You did not select a card to play.");

                    gameModel.CardSelected = new Card();
                    gameModel.CardSelected.ConvertStringToCard(gameModel.CardSelectedString);

                    if (cardsInHand.Find(x => x.Suit == gameModel.CardSelected.Suit && x.Value == gameModel.CardSelected.Value) == null)
                        throw new CustomException(String.Format("{0} Invalid card. You do not hve this card in your hand", gameModel.CardSelected.Name));

                    //check if card is a valid move
                    if (game.LeadingSuit != null)
                    {
                        //if player doesnt play leading suit
                        if ((int)gameModel.CardSelected.Suit != game.LeadingSuit.Value)
                        {
                            //check if cardsinHand have leading suit
                            if (cardsInHand.FirstOrDefault(x => x.Suit == (Suit)game.LeadingSuit) != null)
                                throw new CustomException("Play the leading suit.");
                        }
                        // else play any suit, card is accepted
                        if (gameModel.CardSelected.Suit == Suit.Hearts)
                            game.HeartsPlayed = true;
                        if (gameModel.CardSelected.Suit == Suit.Spades)
                            game.SpadesPlayed = true;
                    }
                    else
                    {
                        if (cardsInHand.Count == 13)
                            if (!(gameModel.CardSelected.Suit == Suit.Clubs && gameModel.CardSelected.Value == Value.Two))
                                throw new CustomException("First Card to the first trick is Two of Clubs");

                        if (game.HeartsPlayed==false && gameModel.CardSelected.Suit == Suit.Hearts)
                            throw new CustomException("You cannot play Hearts as a leading suit, untill Hearts are broken.");
                        if (game.SpadesPlayed == false && gameModel.CardSelected.Suit == Suit.Spades)
                            throw new CustomException("You cannot play Spades as a leading suit, untill Spades are broken.");

                        //update leading suit
                        game.LeadingSuit = (int)gameModel.CardSelected.Suit;
                    }

                    //remove selected card from hand
                    cardsInHand.RemoveAll(x => x.Suit == gameModel.CardSelected.Suit && x.Value == gameModel.CardSelected.Value);

                    //save selected card & update the turn & update stash
                    if (game.Player1 == userId)
                    {
                        game.Player1Card = gameModel.CardSelected.Value+"-"+ gameModel.CardSelected.Suit;
                        game.Turn = game.Player2;
                    }
                    else if (game.Player2 == userId)
                    {
                        game.Player2Card = gameModel.CardSelected.Value + "-" + gameModel.CardSelected.Suit;
                        game.Turn = game.Player3;
                    }
                    else if (game.Player3 == userId)
                    {
                        game.Player3Card = gameModel.CardSelected.Value + "-" + gameModel.CardSelected.Suit;
                        game.Turn = game.Player4;
                    }
                    else if (game.Player4 == userId)
                    {
                        game.Player4Card = gameModel.CardSelected.Value + "-" + gameModel.CardSelected.Suit;
                        game.Turn = game.Player1;
                    }

                    //if all players played, update scores
                    if (game.Player1Card != null && game.Player2Card != null &&
                        game.Player3Card != null && game.Player4Card != null)
                    {
                        if (game.Player1Card.Length > 0 && game.Player2Card.Length > 0 &&
                        game.Player3Card.Length > 0 && game.Player4Card.Length > 0)
                        {
                            //get all cards playeds
                            Card player1Card = new Card();
                            player1Card.ConvertStringToCard(game.Player1Card);
                            Card player2Card = new Card();
                            player2Card.ConvertStringToCard(game.Player2Card);
                            Card player3Card = new Card();
                            player3Card.ConvertStringToCard(game.Player3Card);
                            Card player4Card = new Card();
                            player4Card.ConvertStringToCard(game.Player4Card);

                            //calculate points
                            int totalPoints = player1Card.Points + player2Card.Points +
                                player3Card.Points + player4Card.Points;

                            //calculate the stash
                            stash = "," + game.Player1Card + "," + game.Player2Card + "," + game.Player3Card + "," + game.Player4Card;

                            //find winner, set next turn and update points
                            Value maxValue = Value.Two;
                            if (player1Card.Suit == (Suit)game.LeadingSuit && player1Card.Value >= maxValue)
                            {
                                maxValue = player1Card.Value;
                                game.Turn = game.Player1.Value;
                            }
                            if (player2Card.Suit == (Suit)game.LeadingSuit && player2Card.Value >= maxValue)
                            {
                                maxValue = player2Card.Value;
                                game.Turn = game.Player2.Value;
                            }
                            if (player3Card.Suit == (Suit)game.LeadingSuit && player3Card.Value >= maxValue)
                            {
                                maxValue = player3Card.Value;
                                game.Turn = game.Player3.Value;
                            }
                            if (player4Card.Suit == (Suit)game.LeadingSuit && player4Card.Value >= maxValue)
                            {
                                maxValue = player4Card.Value;
                                game.Turn = game.Player4.Value;
                            }

                            if (game.Turn == game.Player1)
                            {
                                game.Player1Score += totalPoints;
                                game.Player1Stash += stash;
                            }
                            else if (game.Turn == game.Player2)
                            {
                                game.Player2Score += totalPoints;
                                game.Player2Stash += stash;
                            }
                            else if (game.Turn == game.Player3)
                            {
                                game.Player3Score += totalPoints;
                                game.Player3Stash += stash;
                            }
                            else if (game.Turn == game.Player4)
                            {
                                game.Player4Score += totalPoints;
                                game.Player4Stash += stash;
                            }

                            //clear leading suit and PlayerCards
                            game.LeadingSuit = null;
                            game.Player1Card = null;
                            game.Player2Card = null;
                            game.Player3Card = null;
                            game.Player4Card = null;
                        }
                    }
                }

                //update cards in hand
                if (game.PassOrPlay == 1 || game.PassOrPlay == 2)
                {
                    string newHand = "";
                    foreach (var card in cardsInHand)
                        newHand = newHand + "," + card.Value + "-" + card.Suit;

                    if (game.Player1.Value == userId)
                    {
                        game.Player1Hand = newHand;
                    }
                    else if (game.Player2.Value == userId)
                    {
                        game.Player2Hand = newHand;
                    }
                    else if (game.Player3.Value == userId)
                    {
                        game.Player3Hand = newHand;
                    }
                    else if (game.Player4.Value == userId)
                    {
                        game.Player4Hand = newHand;
                    }
                    
                    if (game.PassOrPlay == 1) // pass the trass
                    {
                        foreach (var card in gameModel.CardsToPass)
                            if (card.IsSlected)
                            {
                                if (game.Player1.Value == userId)
                                    game.Player1Trash = game.Player1Trash + "," + card.Value + "-" + card.Suit;
                                else if (game.Player2.Value == userId)
                                    game.Player2Trash = game.Player2Trash + "," + card.Value + "-" + card.Suit;
                                else if (game.Player3.Value == userId)
                                    game.Player3Trash = game.Player3Trash + "," + card.Value + "-" + card.Suit;
                                else if (game.Player4.Value == userId)
                                    game.Player4Trash = game.Player4Trash + "," + card.Value + "-" + card.Suit;
                            }
                    }
                    gdal.UpdateGameAfterMove(game, userId);

                    string winner=null;

                    // if PassOrPlay == 2 && all cards of all the players are over
                    // call Inititalize round function
                    if (game.PassOrPlay == 2) // play 
                    {
                        if (game.Player1Hand.Length == 0 && game.Player2Hand.Length == 0
                            && game.Player1Hand.Length == 0 && game.Player1Hand.Length == 0)
                        {
                            if (game.GameRound == 4)
                            {
                                winner = UpdateScores(gameModel);
                            }
                            InitializeRound(game.GameId);
                        }
                    }
                    var returnModel = GetGame(gameModel.GameURL, hashedUserId);
                    returnModel.Winner = winner;
                    return returnModel;
                }
                return null;
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public GameModel GetGame(string hashedGameId, string hashedUserId)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                CustomCrypto unhash = new CustomCrypto();

                var g = unhash.Decrypt(hashedGameId);
                int gameId = Convert.ToInt32(g);
                int userId = Convert.ToInt32(unhash.Decrypt(hashedUserId));

                var game = gdal.GetGame(gameId);
                var user = udal.GetUserById(userId);

                if (game.Player1.Value != user.UserId &&
                    game.Player2.Value != user.UserId &&
                    game.Player3.Value != user.UserId &&
                    game.Player4.Value != user.UserId)
                    throw new CustomException("You cannot access this game currently. Join or create another game.");

                if (game.Status == (int)GameStatus.Aborted || game.Status == (int)GameStatus.Ended)
                    throw new CustomException("Game has either aborted or ended.");

                //resume a started or waiting game
                GameModel gameModel = new GameModel
                {
                    GameURL = unhash.Encrypt(game.GameId.ToString()),
                    Status = game.Status,
                    EndTime = game.EndTime,
                    StartTime = game.StartTime
                };
                User player;
                if (game.Player1 != null)
                {
                    player = udal.GetUserById(game.Player1.Value);
                    gameModel.Player1 = new Player(player.UserId, player.Username);
                }
                if (game.Player2 != null)
                {
                    player = udal.GetUserById(game.Player2.Value);
                    gameModel.Player2 = new Player(player.UserId, player.Username);
                }
                if (game.Player3 != null)
                {
                    player = udal.GetUserById(game.Player3.Value);
                    gameModel.Player3 = new Player(player.UserId, player.Username);
                }
                if (game.Player4 != null)
                {
                    player = udal.GetUserById(game.Player4.Value);
                    gameModel.Player4 = new Player(player.UserId, player.Username);
                }

                //return game status to player resuming the game
                if (game.Status == (int)GameStatus.Started)
                {
                    if (game.GameRound != null)
                        gameModel.GameRound = game.GameRound.Value;
                    if (game.PassOrPlay != null)
                        gameModel.PassOrPlay = game.PassOrPlay.Value;
                    if (game.Turn != null)
                        gameModel.PlayerTurn = game.Turn.Value;
                    gameModel.CurrentTurn = udal.GetUserById(gameModel.PlayerTurn).Username;

                    gameModel.HeartsPlayed = game.HeartsPlayed;
                    gameModel.SpadesPlayed = game.SpadesPlayed;


                    if (game.LeadingSuit != null)
                    {
                        gameModel.Trick = new MoveModel();
                        gameModel.Trick.LeadingSuit = (Suit)(game.LeadingSuit.Value);
                        if (game.Player1Card != null)
                        {
                            gameModel.Trick.Card1 = new Card();
                            gameModel.Trick.Card1.ConvertStringToCard(game.Player1Card);
                        }
                        if (game.Player2Card != null)
                        {
                            gameModel.Trick.Card2 = new Card();
                            gameModel.Trick.Card2.ConvertStringToCard(game.Player2Card);
                        }
                        if (game.Player3Card != null)
                        {
                            gameModel.Trick.Card3 = new Card();
                            gameModel.Trick.Card3.ConvertStringToCard(game.Player3Card);
                        }
                        if (game.Player4Card != null)
                        {
                            gameModel.Trick.Card4 = new Card();
                            gameModel.Trick.Card4.ConvertStringToCard(game.Player4Card);
                        }
                    }
                    gameModel.SpadesPlayed = game.SpadesPlayed;
                    gameModel.HeartsPlayed = game.HeartsPlayed;

                    if (game.Player1 == userId)
                    {
                        gameModel.Player1.Hand = ConvertStringtoListCard(game.Player1Hand, game.Player1.Value);
                    }
                    else if (game.Player2 == userId)
                    {
                        gameModel.Player2.Hand = ConvertStringtoListCard(game.Player2Hand, game.Player2.Value);
                    }
                    else if (game.Player3 == userId)
                    {
                        gameModel.Player3.Hand = ConvertStringtoListCard(game.Player3Hand, game.Player3.Value);
                    }
                    else if (game.Player4 == userId)
                    {
                        gameModel.Player4.Hand = ConvertStringtoListCard(game.Player4Hand, game.Player4.Value);
                    }
                    gameModel.Player1.Stash = ConvertStringtoListCard(game.Player1Stash, game.Player1.Value);
                    gameModel.Player2.Stash = ConvertStringtoListCard(game.Player2Stash, game.Player2.Value);
                    gameModel.Player3.Stash = ConvertStringtoListCard(game.Player3Stash, game.Player3.Value);
                    gameModel.Player4.Stash = ConvertStringtoListCard(game.Player4Stash, game.Player4.Value);

                    gameModel.Player1.PreviousRoundPoint = game.Player1Score;
                    gameModel.Player2.PreviousRoundPoint = game.Player2Score;
                    gameModel.Player3.PreviousRoundPoint = game.Player3Score;
                    gameModel.Player4.PreviousRoundPoint = game.Player4Score;
                }
                return gameModel;
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public List<GameModel> GetAllWaitingGames()
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                CustomCrypto hashing = new CustomCrypto();

                var games = gdal.GetAllWaitingGames();
                List<GameModel> waiting_game = new List<GameModel>();

                foreach (var game in games)
                {
                    var new_game = new GameModel
                    {
                        //GameId = game.GameId,
                        GameURL = hashing.Encrypt(game.GameId.ToString()),
                        Status = game.Status,
                        EndTime = game.EndTime,
                        StartTime = game.StartTime
                    };
                    User player;
                    if (game.Player1 != null)
                    {
                        player = udal.GetUserById(game.Player1.Value);
                        new_game.Player1 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player2 != null) {
                        player = udal.GetUserById(game.Player2.Value);
                        new_game.Player2 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player3 != null)
                    {
                        player = udal.GetUserById(game.Player3.Value);
                        new_game.Player3 = new Player(player.UserId, player.Username);
                    }
                    if (game.Player4 != null)
                    {
                        player = udal.GetUserById(game.Player4.Value);
                        new_game.Player4 = new Player(player.UserId, player.Username);
                    }
                    waiting_game.Add(new_game);
                }
                return waiting_game;

            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }

        }

        private void WaitForPlayers(int seconds, int gameId)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }

        public bool AbortGame(string hashedgameId)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                CustomCrypto unhash = new CustomCrypto();
                int gameId = Convert.ToInt32(unhash.Decrypt(hashedgameId));
                return gdal.AbortGame(gameId);
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        private void InitializeGame(int gameId)
        {
            GameModel gameModel = new GameModel();
            gameModel.Player1 = new Player();
            gameModel.Player2 = new Player();
            gameModel.Player3 = new Player();
            gameModel.Player4 = new Player();

            gameModel.InitializeDeck();
            gameModel.DealCards();

            var g_dal = new GameDAL();
            var game = g_dal.GetGame(gameId);
            game.GameId = gameId;
            game.Status = (int)GameStatus.Started;
            game.PassOrPlay = 1;
            game.GameRound = 1;
            game.LeadingSuit = null;
            game.HeartsPlayed = false;
            game.SpadesPlayed = false;


            if (game.Player1!=null)
                foreach (var card in gameModel.Player1.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player1.Value;
                    game.Player1Hand = game.Player1Hand + "," + card.Value + "-" + card.Suit;
                }
            if (game.Player2 != null)
                foreach (var card in gameModel.Player2.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player2.Value;
                    game.Player2Hand = game.Player2Hand + "," + card.Value + "-" + card.Suit;
                }
            if (game.Player3 != null)
                foreach (var card in gameModel.Player3.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player3.Value;
                    game.Player3Hand = game.Player3Hand + "," + card.Value + "-" + card.Suit;
                }
            if (game.Player4 != null)
                foreach (var card in gameModel.Player4.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player4.Value;
                    game.Player4Hand = game.Player4Hand + "," + card.Value + "-" + card.Suit;
                }

            
            g_dal.UpdateGameAfterInit(game);
        }

        private void InitializeRound(int gameId) {
            GameModel gameModel = new GameModel();
            gameModel.Player1 = new Player();
            gameModel.Player2 = new Player();
            gameModel.Player3 = new Player();
            gameModel.Player4 = new Player();

            gameModel.InitializeDeck();
            gameModel.DealCards();

            Game game = new GameDAL().GetGame(gameId);
            game.Status = (int)GameStatus.Started;
            game.GameRound++;

            game.LeadingSuit = null;

            game.HeartsPlayed = false;
            game.SpadesPlayed = false;

            game.Player1Stash = null;
            game.Player2Stash = null;
            game.Player3Stash = null;
            game.Player4Stash = null;

            game.Player1Card = null;
            game.Player2Card = null;
            game.Player3Card = null;
            game.Player4Card = null;

            game.Player1Trash = null;
            game.Player2Trash = null;
            game.Player3Trash = null;
            game.Player4Trash = null;

            if (game.GameRound == 5)
                AbortGame(new CustomCrypto().Encrypt(game.GameId.ToString()));
            else
            {
                if (game.GameRound == 4)
                    game.PassOrPlay = 2;
                else
                    game.PassOrPlay = 1;

                foreach (var card in gameModel.Player1.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player1.Value;
                    game.Player1Hand = game.Player1Hand + "," + card.Value + "-" + card.Suit;
                }
                foreach (var card in gameModel.Player2.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player2.Value;
                    game.Player2Hand = game.Player2Hand + "," + card.Value + "-" + card.Suit;
                }
                foreach (var card in gameModel.Player3.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player3.Value;
                    game.Player3Hand = game.Player3Hand + "," + card.Value + "-" + card.Suit;
                }
                foreach (var card in gameModel.Player4.Hand)
                {
                    if (card.Suit == Suit.Clubs && card.Value == Value.Two)
                        game.Turn = game.Player3.Value;
                    game.Player4Hand = game.Player4Hand + "," + card.Value + "-" + card.Suit;
                }

                var g_dal = new GameDAL();
                g_dal.UpdateGameAfterInit(game);
            }
        }

        private List<Card> ConvertStringtoListCard(string listString, int playerId)
        {
            List<Card> cards = new List<Card>();
            if (listString != null)
            {
                if (listString.Length > 0)
                {
                    listString = listString.Substring(1, listString.Length-1);
                    string[] cardString = listString.Split(',');

                    foreach (var card in cardString)
                    {
                        Card cardObj = new Card();
                        cardObj.ConvertStringToCard(card);
                        cardObj.PlayerId = playerId;
                        cards.Add(cardObj);
                    }
                }
                
            }
            return cards;
        }

        private string UpdateScores(GameModel gameModel)
        {
            GameDAL gdal = new GameDAL();
            var game = new Game();
            game.Status = (int)GameStatus.Ended;
            game.Player1Score = gameModel.Player1.Points;
            game.Player2Score = gameModel.Player2.Points;
            game.Player3Score = gameModel.Player3.Points;
            game.Player4Score = gameModel.Player4.Points;
            return gdal.UpdateScores(game);
        }

    }
}
