using ClientServerLibrary;
using System;
using System.Collections.Generic;

namespace GameObjects.Models
{
    /// <summary>
    /// Generic game class for the game.  Provides basic functionality.
    /// </summary>
    /// 
    [Serializable]
    public class GameModel
    {
        #region Private Properties
        private static int _nextId = 0;

        #endregion

        #region Public Properties
        /// <summary>
        /// The game ID for this game
        /// </summary>
        public int GameId { get; private set; } 

        /// <summary>
        /// Flag to determine if the game is over
        /// </summary>
        public bool GameIsOver { get; set; } = false;

        public bool GameHasStarted { get; set; } = false;

        public bool GameIsPaused { get; set; } = false;

        /// <summary>
        /// Flag that indicates that the current player has completed their tur.
        /// </summary>
        public bool TurnComplete { get; set; } = false;

        /// <summary>
        /// Flag that indicates if the gameboard needs to be checked for valid moves.
        /// This is reset at the start of each player turn;
        /// </summary>
        public bool GameOverCheckRequired { get; set; } = true;

        /// <summary>
        /// The current player whoss turn is active as indexed by position in
        /// the CurrentPlayerList collection.  Default is zero.
        /// </summary>
        public Players CurrentPlayer { get; set; } = Players.Player1;

        /// <summary>
        /// The index of the current move
        /// </summary>
        public int CurrentMoveIndex { get; set; } = -1;


        /// <summary>
        /// A list of the players for our game
        /// </summary>
        public Dictionary<int, ClientModel> CurrentPlayersList { get; set; }

        /// <summary>
        /// The gameboard for our game
        /// </summary>
        public BoardModel Gameboard { get; set; }


        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for a game object
        /// </summary>
        /// <param name="list">List of players to join the game</param>
        public GameModel(int rows, int cols)
        {
            // Set our game id
            GameId = NextId();

            // create our gameboard
            Gameboard = new BoardModel(cols, rows);


        }

        ///// <summary>
        ///// The constructor for a game object
        ///// </summary>
        ///// <param name="list">List of players to join the game</param>
        //public GameModel(List<PlayerModel> list)
        //{
        //    // Set our game id
        //    GameId = NextId();

        //    // create our gameboard
        //    Gameboard = new BoardModel(8, 8);

        //    // Initialize our players list
        //    CurrentPlayersList = new PlayerModel[list.Count];

        //    for(int i=0; i<list.Count; i++)
        //    {
        //        // create our empty players
        //        CurrentPlayersList[i] = list[i];
        //    }
        //}

        #endregion

        #region Virtual Methods
        /// <summary>
        /// Toggle the current and opponent player properties for the game.
        /// Currently assumes only two players per game.
        /// </summary>
        public virtual void NextPlayer()
        {
            Players old = CurrentPlayer;
            if (CurrentPlayer == Players.Player1)
                CurrentPlayer = Players.Player2;
            else
            {
                CurrentPlayer = Players.Player1;
            }
        }

        public virtual bool ValidateTilePlacement(int index, Players p)
        {
            return false;

        }

        public virtual bool CheckGameOver(Players p)
        {
            return true;
        }

        public virtual void StartGame() {}

        public virtual bool PlayTurn()
        {
            return false;

        }
        public virtual void PlayRound() {}

        public virtual void SetupGame() {}

        public virtual bool CheckGameOver()
        {
            return false;
        }

        #endregion

        #region Private Methods
        private int NextId()
        {
            _nextId++;
            return _nextId;
        }
        
        #endregion

        #region Public Methods
 
        ///// <summary>
        ///// Returns a player from the current list of current game players
        ///// </summary>
        ///// <param name="id">The player id</param>
        ///// <returns></returns>
        //public PlayerModel GetPlayerById(int id)
        //{
        //    foreach(ClientModel item in CurrentPlayersList)
        //    {
        //        if (item.PlayerId == id)
        //            return item;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Removes a player from the current players list
        /// </summary>
        /// <param name="clientModel"></param>
        public virtual void RemovePlayerFromGame(ClientModel clientModel)
        {
            foreach (KeyValuePair<int, ClientModel> item in CurrentPlayersList)
            {
                if (item.Key == clientModel.Id)
                {
                    CurrentPlayersList.Remove(item.Key);
                    break;
                }

                GameIsPaused = true;

            }
        }


        public string DisplayGameStats()
        {
            string str = string.Empty;
            str += "Number of Players: " + CurrentPlayersList.Count + "\n";
            str += "Current Turn: " + CurrentPlayer + "\n";
            str += " ---------------------------------------------------\n";

            return str;
        }

        public virtual string DisplayGameInfoComplete()
        {
            string str = string.Empty;
            str += "----- GAME INFO -----\n";
            str += DisplayGameStats() + "\n";
            str += Gameboard.DisplayGameboardStats() + "\n";
            return str;
        }




        #endregion
    }
}
