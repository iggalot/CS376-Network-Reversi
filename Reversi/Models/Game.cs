using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Reversi.Models
{
    /// <summary>
    /// The Reversi Game class...
    /// </summary>
    [Serializable]
    public class ReversiGame : Game
    {
        #region Public Properties
        /// <summary>
        /// An array to hold the indices of tiles to turn in a given round.
        /// </summary>
        public List<int> TilesToTurn { get; set; } = new List<int>();

        /// <summary>
        /// Has the P1 player gone yet?
        /// </summary>
        public bool P1Went { get; set; } = false;

        /// <summary>
        /// Has the P2 player gone yet?
        /// </summary>
        public bool P2Went { get; set; } = false;
        #endregion

        #region Constructors

        public ReversiGame(int num_players) : base(num_players)
        {
            // Setup the gameboard
            SetupGame();

            // Start the game
            PlayGame();
        }

        /// <summary>
        /// Constructor for a two player game
        /// </summary>
        /// <param name="p1">Player 1</param>
        /// <param name="p2">Player 2</param>
        public ReversiGame(Player p1, Player p2) : base(p1, p2)
        {
            // Setup the gameboard
            SetupGame();

            // Start the game
            PlayGame();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine if the new placement location is valid.
        /// </summary>
        /// <param name="index">The square index where they are placing the piece</param>
        /// <returns></returns>
        internal bool ValidatePlacement(int index)
        {
            bool isValidMove = false;

            Console.WriteLine("Index: " + index);
            // Determine our opponent
            int player = CurrentPlayer;
            int opponent = (player == CurrentPlayers[0].PlayerID) ? CurrentPlayers[1].PlayerID : CurrentPlayers[0].PlayerID;

            // Make sure that we are within acceptable index ranges, otherwise return
            if (index < 0 || index >= Gameboard.Squares.Length)
            {
                return false;
            }
            // Selected index must not already contain a piece
            if (Gameboard.Squares[index].Piece != null)
            {
                //MessageBox.Show("This block already contains a piece.");
                return false;
            }

            var tmp_index = index;
            // 1 2 3
            // 4 X 5
            // 6 7 8
            // A neightbor must be owned by the opposite player
            foreach (DirectionVectors dv in Enum.GetValues(typeof(DirectionVectors)))
            {
                isValidMove = false;

                tmp_index = Gameboard.GetIndexByOffsets(index, dv);

                // Is there a valid piece in this square?
                if ((tmp_index == -1) || (Gameboard.Squares[tmp_index].Piece == null))
                    continue;

                // Check if the neighbor is owned by the opponent
                Player owner = Gameboard.Squares[tmp_index].Piece.Owner;

                if (owner.PlayerID != opponent)
                {
                    // Neighbor isn't the opponent so not a valid move
                    continue;
                }
                else
                {
                    // Store indices in a list while we search
                    List<int> tmpList = new List<int>();

                    // Otherwise continue searching in this direction to see if a 
                    // players piece is also in this direction.
                    int next_index = tmp_index;
                    Player next_owner = owner;

                    // Continue searching so long as we don't reach the border (-1) and the next square is 
                    // owned by the opponent.
                    while ((next_index != -1) && (next_owner.PlayerID == opponent))
                    {
                        // Add our element to the list.
                        tmpList.Add(next_index);

                        Console.WriteLine("...searching " + next_index + " to " + dv);

                        next_index = Gameboard.GetIndexByOffsets(next_index, dv);


                        // Did we find the border? Is there a valid piece in this square? 
                        // If not, stop searching
                        if ((next_index == -1) || (Gameboard.Squares[next_index].Piece == null))
                        {
                            tmpList.Clear(); // clear the temp list
                            break;
                        }

                        next_owner = Gameboard.Squares[next_index].Piece.Owner;

                        // If neighbor to the neighbor in this direction is the same as the player,
                        // the move is valid.
                        if (next_owner.PlayerID == player)
                        {
                            isValidMove = true;
                            break;
                        }
                    }
                    if (isValidMove)
                    {
                        Console.WriteLine("...VALID to " + dv);
                        foreach (int item in tmpList)
                            TilesToTurn.Add(item);
                        tmpList.Clear();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Primary routine for a player to make a move
        /// </summary>
        /// <param name="index">the index of the move being made</param>
        internal void MakePlayerMove(int index)
        {
            Player player = null; ;
            bool playerFound = false;            

            foreach(Player item in CurrentPlayers)
            {
                if (item.PlayerID == CurrentPlayer)
                {
                    player = item;
                    playerFound = true;
                }                    
            }

            if (playerFound && ValidatePlacement(index))
            {
                if (player == null)
                    return;

                // Add a new game piece at the location
                GamePiece piece = new GamePiece(Pieceshapes.ELLIPSE, player);
                Gameboard.AddPiece(index, piece);

                // Capture the opponents tiles
                DoTurnTiles();

                // Reset the tiles to be turned array
                TilesToTurn.Clear();
            }
            //MessageBox.Show(Gameboard.DrawGameboard() + "\nCurrent Player: " + CurrentPlayer.IDType + " : " + CurrentPlayer.Name);
        }

        /// <summary>
        /// Routine that changes the owner of a tile based on indices stored in TilesToTurn list.
        /// </summary>
        private void DoTurnTiles()
        {
            Player player = null; ;
            bool playerFound = false;

            foreach (Player item in CurrentPlayers)
            {
                if (item.PlayerID == CurrentPlayer)
                {
                    player = item;
                    playerFound = true;
                }
            }

            // If we didnt find the player, return
            if (!playerFound)
                return;

            foreach (int index in TilesToTurn)
            {
                Gameboard.Squares[index].Piece.Owner = player;

                //// For each tile being flipped...play a sounds
                //ReversiSounds.PlaySounds(GameSounds.SOUND_FLIPTILE);
            }

            // Now clear the tiles to turn array since all the moves have been made
            TilesToTurn.Clear();
        }

        public void SetupGame()
        {
            // TODO:  What if the board has odd rows and columns ODD numbered?
            var midpoint = Gameboard.Rows * Gameboard.Cols / 2;

            // Place the starting pieces
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2 - 1, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[0]));
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2 - 1, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[0]));

            // MessageBox.Show(Gameboard.DrawGameboard());

            // Set the First player to be the current player
            CurrentPlayer = CurrentPlayers[0].PlayerID;
        }


        /// <summary>
        /// The main game routine
        /// </summary>
        public void PlayGame()
        {
            // Player 1

            // Test the boundary detection cases
            //MakePlayerMove(Players[0], 0);
            //MakePlayerMove(Players[0], 1);
            //MakePlayerMove(Players[0], 62);
            //MakePlayerMove(Players[0], 63);

            // Add a piece to test a valid move
            //            Gameboard.AddPiece(20, Gameboard.Squares[28].Piece);
            //            MakePlayerMove(Players[0], 12);

            // Test scenario for our board
            Gameboard.AddPiece(18, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            Gameboard.AddPiece(17, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[0]));
            Gameboard.AddPiece(19, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            Gameboard.AddPiece(21, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            Gameboard.AddPiece(12, new GamePiece(Pieceshapes.ELLIPSE, CurrentPlayers[1]));
            //MessageBox.Show(Gameboard.DrawGameboard());

            //// The main game loop -- 
            //while (!IsGameOver)
            //{
            //    // Play a round for the current player
            //    PlayRound();

            //    // Cycle to next player
            //    NextPlayer();

            //    IsGameOver = CheckGameOver();
            //}

        }

        /// <summary>
        /// Checks if there is a valid move for the current player.
        /// </summary>
        /// <returns></returns>
        private bool CheckGameOver()
        {
            for (int i = 0; i < Gameboard.Rows * Gameboard.Cols; i++)
            {
                if (!ValidatePlacement(i))
                {
                    return false;
                }
            }

            return true;
        }

        public void PlayRound()
        {
            // Make a move for player 1
            MakePlayerMove(CurrentMoveIndex);
            NextPlayer();
        }

        #endregion

    }

    /// <summary>
    /// Generic game class for the game.  Provides basic functionality.
    /// </summary>
    /// 

    [Serializable]
    public class Game
    {
        #region Private Properties
        private static int _nextId = 0;

        #endregion

        #region Public Properties
        /// <summary>
        /// The game IDType for this game
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// Flag to determine if the game is over
        /// </summary>
        public bool GameIsOver { get; set; } = false;

        /// <summary>
        /// The current player whos turn is active
        /// </summary>
        public int CurrentPlayer { get; set; }

        /// <summary>
        /// The index of the current move
        /// </summary>
        public int CurrentMoveIndex { get; set; } = 20;


        /// <summary>
        /// A list of the players for our game
        /// </summary>
        public Player[] CurrentPlayers { get; set; }

        /// <summary>
        /// The gameboard for our game
        /// </summary>
        public Board Gameboard { get; set; }


        #endregion

        #region Constructor
        public Game(int num_players)
        {
            // Set our game id
            GameID = NextID();

            // create our gameboard
            Gameboard = new Board(8, 8);

            // Initialize our players list
            CurrentPlayers = new Player[num_players];

            // create our empty players
            CurrentPlayers[0] = new Player();
            CurrentPlayers[1] = new Player();
        }

        /// <summary>
        /// Constructor that creates a game of two players.
        /// </summary>
        /// <param name="p1">Player 1</param>
        /// <param name="p2">Player 2</param>
        public Game(Player p1, Player p2)
        {
            // Set our game id
            GameID = NextID();

            // create our gameboard
            Gameboard = new Board(8, 8);

            // Initialize our players list
            CurrentPlayers = new Player[2];

            // Assign the players
            CurrentPlayers[0] = p1;
            CurrentPlayers[1] = p2;
        }

        #endregion

        #region Private Methods
        private int NextID()
        {
            _nextId++;
            return _nextId;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Toggle the current and opponent player properties for the game
        /// </summary>
        public void NextPlayer()
        {
            CurrentPlayer = (CurrentPlayer == CurrentPlayers[0].PlayerID) ? CurrentPlayers[1].PlayerID : CurrentPlayers[0].PlayerID;
        }

        /// <summary>
        /// Returns a player from the current list of current game players
        /// </summary>
        /// <param name="id">The player id</param>
        /// <returns></returns>
        public Player GetPlayerById(int id)
        {
            foreach(Player item in CurrentPlayers)
            {
                if (item.PlayerID == id)
                    return item;
            }

            return null;

        }

        /// <summary>
        /// Make a list of the current player sockets for the game.
        /// </summary>
        /// <returns></returns>
        public List<TcpClient> GetPlayersSocketList()
        {
            List<TcpClient> list = new List<TcpClient>();

            foreach(Player item in CurrentPlayers)
            {
                list.Add(item.Socket);
            }

            return list;
        }
        #endregion
    }
}
