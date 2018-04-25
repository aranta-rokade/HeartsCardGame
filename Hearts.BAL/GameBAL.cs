using Hearts.DAL;
using Hearts.ViewModel;
using System;
using System.Collections.Generic;
using Utility;

namespace Hearts.BAL
{
    public class GameBAL
    {
        public string NewGame(string hashedPlayerId)
        {
            try
            {
                Hashing hashing = new Hashing();
                int playerId = Convert.ToInt32(hashing.Decrypt(hashedPlayerId));
                GameDAL gdal = new GameDAL();
                int gameId = gdal.AddGame(playerId);
                var hashed_gameId = hashing.Encrypt(gameId.ToString());
                // Wait for other players to join
                WaitForPlayers(90, gameId);
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
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public string JoinGame(string hashedGameId, string hashedPlayerid)
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                Hashing hashing = new Hashing();

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
                    time = 90 - secondsElapsed;
                }
                else if (game.Player2 == null)
                {
                    game.Player2 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }
                else if (game.Player3 == null)
                {
                    game.Player3 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }
                else if (game.Player4 == null)
                {
                    game.Player4 = playerid;
                    var secondsElapsed = (DateTime.Now).Subtract(game.StartTime).Seconds;
                    time = 90 - secondsElapsed;
                }

                var new_game = gdal.AddPlayer(playerid, game.GameId);
                udal.UpdateActiveGame(playerid, new_game.GameId);

                WaitForPlayers(time, new_game.GameId);

                game = gdal.GetGame(game.GameId);

                // check if game room is full
                if (game.Player1 != null && game.Player2 != null && game.Player3 != null && game.Player4 != null)
                {
                    InitializeGame(game.GameId);
                    return hashing.Encrypt(new_game.GameId.ToString());
                }

                AbortGame(hashedGameId);
                throw new CustomException("Timeout. Players insufficient.");

            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }

        public GameModel GetGame(string hashedGameId, string hashedUserId)
        {
            GameDAL gdal = new GameDAL();
            UserDAL udal = new UserDAL();
            Hashing unhash = new Hashing();

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
                if(game.GameRound!=null)
                gameModel.GameRound = game.GameRound.Value;
                if (game.Turn != null)
                    gameModel.PlayerTurn = game.Turn.Value;
                if (game.PassOrPlay != null)
                    gameModel.PassOrPlay = game.PassOrPlay.Value;

                if (game.LeadingSuit != null) {
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
                    
                if (game.Player1 == userId)
                {
                    gameModel.Player1.Hand = ConvertStringtoListCard(game.Player1Hand, game.Player1.Value);
                    gameModel.Player1.Stash = ConvertStringtoListCard(game.Player1Stash, game.Player1.Value);
                }
                else if (game.Player2 == userId)
                {
                    gameModel.Player2.Hand = ConvertStringtoListCard(game.Player2Hand, game.Player2.Value);
                    gameModel.Player2.Stash = ConvertStringtoListCard(game.Player2Stash, game.Player2.Value);
                }
                else if (game.Player3 == userId)
                {
                    gameModel.Player3.Hand = ConvertStringtoListCard(game.Player3Hand, game.Player3.Value);
                    gameModel.Player3.Stash = ConvertStringtoListCard(game.Player3Stash, game.Player3.Value);
                }
                else if (game.Player4 == userId)
                {
                    gameModel.Player4.Hand = ConvertStringtoListCard(game.Player4Hand, game.Player4.Value);
                    gameModel.Player4.Stash = ConvertStringtoListCard(game.Player4Stash, game.Player4.Value);
                }
            }
            return gameModel;
        }

        public List<GameModel> GetAllWaitingGames()
        {
            try
            {
                GameDAL gdal = new GameDAL();
                UserDAL udal = new UserDAL();
                Hashing hashing = new Hashing();

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
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }

        }

        public void WaitForPlayers(int seconds, int gameId)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }

        public bool AbortGame(string hashedgameId)
        {
            GameDAL gdal = new GameDAL();
            Hashing unhash = new Hashing();
            int gameId = Convert.ToInt32(unhash.Decrypt(hashedgameId));
            return gdal.AbortGame(gameId);

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

            Game game = new Game();
            game.GameId = gameId;
            game.Status = (int)GameStatus.Started;
            game.PassOrPlay = 1;
            game.GameRound = 1;

            foreach (var card in gameModel.Player1.Hand)
            {
                game.Player1Hand = game.Player1Hand + "," + card.Value + "-" + card.Suit;
            }
            foreach (var card in gameModel.Player2.Hand)
            {
                game.Player2Hand = game.Player2Hand + "," + card.Value + "-" + card.Suit;
            }
            foreach (var card in gameModel.Player3.Hand)
            {
                game.Player3Hand = game.Player3Hand + "," + card.Value + "-" + card.Suit;
            }
            foreach (var card in gameModel.Player4.Hand)
            {
                game.Player4Hand = game.Player4Hand + "," + card.Value + "-" + card.Suit;
            }

            var g_dal = new GameDAL();
            g_dal.UpdateGameAfterMove(game);
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
    }
}
