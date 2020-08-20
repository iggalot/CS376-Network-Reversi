using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ClientServerLibrary;
using GameObjects.Models;

namespace Reversi.Models
{
    /// <summary>
    /// The Reversi Game class...
    /// </summary>
    [Serializable]
    public class ReversiGameModel : GameModel
    {
        #region Public Properties
        /// <summary>
        /// An array to hold the indices of tiles to turn in a given round.
        /// </summary>
        /// 
        public string TestGameText {get; set;} = "My test game text";
        public List<int> TilesToTurn { get; set; } = new List<int>();

        /// <summary>
        /// Array containing available moves
        /// </summary>
        public List<int> AvailableMovesList { get; set; } = new List<int>();

        /// <summary>
        /// Has the P1 player gone yet?
        /// </summary>
        public bool P1Went { get; set; } = false;

        /// <summary>
        /// Has the P2 player gone yet?
        /// </summary>
        public bool P2Went { get; set; } = false;
        #endregion

        #region Constructor

        public ReversiGameModel(List<ReversiClientModel> list) : base(ReversiSettings.ReversiBoardSizeCols, ReversiSettings.ReversiBoardSizeRows)
        {

            // Initialize our players list
            CurrentPlayersList = new Dictionary<int, ClientModel>();

            for (int i = 0; i < list.Count; i++)
            {
                // set the player numbers
                if (i % 2 == 0)
                    ((ReversiClientModel) list[i]).ClientPlayer.IdType = Players.Player1;
                else if (i % 2 == 1)
                    ((ReversiClientModel)list[i]).ClientPlayer.IdType = Players.Player2;
                else
                {
                    ((ReversiClientModel)list[i]).ClientPlayer.IdType = Players.Undefined;
                }

                CurrentPlayersList.Add(i, list[i]);
            }

            // Setup the gameboard
            SetupGame();

            // Start the game
            StartGame();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Toggle the current and opponent player properties for the game
        /// </summary>
        public override void NextPlayer()
        {
            base.NextPlayer();
        }

        /// <summary>
        /// Check the initial positioning including:
        /// 1.  Does the square have a piece
        /// 2.  Is the index within the gameboard region
        /// </summary>
        /// <param name="index">The index of the piece placement</param>
        /// <param name="p">The player placing the piece</param>
        /// <returns></returns>
        private bool ValidateInitialPlacementPosition(int index, Players p)
        {
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

            return true;
        }

        // Check Direction from Index
        /// <summary>
        /// Check Direction from a specified index value
        /// </summary>
        /// <param name="index">The index where the tile is placed</param>
        /// <param name="p">The player making the move</param>
        /// <param name="opp">The opponent of the player</param>
        /// <param name="d">The direction to check <see cref="DirectionVectors"/></param>
        /// <returns></returns>
        private bool ValidateDirectionFromIndex(int index, Players p, Players opp, DirectionVectors dv, 
            out List<int> result)
        {
            result = new List<int>();
            Players player = p;
            Players opponent = (player == Players.Player1) ? Players.Player2 : Players.Player1;

            // returns -1 if the neighbor is a boundary
            int tmp_index = Gameboard.GetIndexByOffsets(index, dv);

            // Is there a valid piece in this square?
            if ((tmp_index == -1) || (Gameboard.Squares[tmp_index].Piece == null))
                return false;

            // Check if the neighbor is owned by the opponent, if not then its not a valid case
            Players owner = Gameboard.Squares[tmp_index].Piece.Owner.IdType;

            if (owner != opponent)
                return false;

            // Otherwise continue searching in this direction to see if a 
            // players piece is also in this direction.
            int nextIndex = tmp_index;
            Players nextOwner = owner;

            // Continue searching so long as we don't reach the border (-1) and the next square is 
            // owned by the opponent.
            while ((nextIndex != -1) && (nextOwner == opponent))
            {
                // Add our element to the list.
                result.Add(nextIndex);

                Console.WriteLine("...searching " + nextIndex + " to " + dv);

                // Get the next neighbor in the direction
                nextIndex = Gameboard.GetIndexByOffsets(nextIndex, dv);

                // Did we find the border? Is there a valid piece in this square? 
                // If not, stop searching
                if ((nextIndex == -1) || (Gameboard.Squares[nextIndex].Piece == null))
                {
                    result.Clear(); // clear the resul list since the direction isnt valid
                    return false;
                }

                nextOwner = Gameboard.Squares[nextIndex].Piece.Owner.IdType;

                // If neighbor to the neighbor in this direction is the same as the player,
                // the move is valid.
                if (nextOwner == player)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determine if the new placement location is valid.
        /// </summary>
        /// <param name="index">The square index where they are placing the piece</param>
        /// <param name="p">The player placing the tile</param>
        /// <returns></returns>
        public override bool ValidateTilePlacement(int index, Players p)
        {
            // Determine our opponent
            Players player = p;
            Players opponent = (player == Players.Player1) ? Players.Player2 : Players.Player1;

            if (ValidateInitialPlacementPosition(index, p) == false)
                return false;

            var tmp_index = index; // returns -1 if the neighbor is a boundary
            
            // 1 2 3
            // 4 X 5
            // 6 7 8
            foreach (DirectionVectors dv in Enum.GetValues(typeof(DirectionVectors)))
            {
                List<int> result = new List<int>(); // stores the values of a valid placement
                if (ValidateDirectionFromIndex(index, player, opponent, dv, out result) == true)
                {
                    foreach (int item in result)
                    {
                        Console.WriteLine("...VALID to " + dv);
                        if (TilesToTurn.Contains(item) == false)
                            TilesToTurn.Add(item);
                    }
                }
            }

            // Print the Tiles to Turn list

            string str = string.Empty;
            foreach (int item in TilesToTurn)
            {
                str += item.ToString() + " ";
            }
            Console.WriteLine(index + ": -- " + str + "\n");

            return (TilesToTurn.Count > 0);
        }

        /// <summary>
        /// Primary routine for a player to make a move
        /// </summary>
        /// <param name="index">the index of the move being made</param>
        internal bool MakePlayerMove(int index)
        {
            PlayerModel player = null; ;
            bool playerFound = false;

            foreach(KeyValuePair<int,ClientModel> model in CurrentPlayersList)
            {
                ReversiClientModel reversiClientModel = (ReversiClientModel) model.Value;

                if (reversiClientModel.ClientPlayer.IdType == CurrentPlayer)
                {
                    player = reversiClientModel.ClientPlayer;
                    playerFound = true;
                    break;
                }
            }

            if ((player == null) || (playerFound == false))
                return false;

            bool placementIsValid = ValidateTilePlacement(index, CurrentPlayer);
            if (placementIsValid)
            {
                // Add a new game piece at the location
                GamePieceModel piece = new GamePieceModel(Pieceshapes.Ellipse, player);
                Gameboard.AddPiece(index, piece);

                // Capture the opponents tiles
                DoTurnTiles();

                // Reset the tiles to be turned array
                TilesToTurn.Clear();
                return true;
            }

            return false;
            //MessageBox.Show(Gameboard.DrawGameboard() + "\nCurrent Player: " + CurrentPlayer.IDType + " : " + CurrentPlayer.Name);
        }

        /// <summary>
        /// Routine that changes the owner of a tile based on indices stored in TilesToTurn list.
        /// </summary>
        private void DoTurnTiles()
        {
            PlayerModel player = null; ;
            bool playerFound = false;

            foreach (KeyValuePair<int,ClientModel> model in CurrentPlayersList)
            {
                ReversiClientModel reversiClientModel = (ReversiClientModel) model.Value;

                if (reversiClientModel.ClientPlayer.IdType == CurrentPlayer)
                {
                    player = reversiClientModel.ClientPlayer;
                    playerFound = true;
                }
            }

            // If we didnt find the player, return
            if (!playerFound)
                return;

            foreach (int index in TilesToTurn)
            {
                Gameboard.Squares[index].Piece.Owner = player;

                //// TODO: For each tile being flipped...play a sounds
                //ReversiSounds.PlaySounds(GameSounds.SOUND_FLIPTILE);
            }

            // Now clear the tiles to turn array since all the moves have been made
            TilesToTurn.Clear();
        }

        public override void SetupGame()
        {
            // TODO:  What if the board has odd rows and columns ODD numbered?
            var midpoint = Gameboard.Rows * Gameboard.Cols / 2;

            //            PlayerModel player1 = ((ReversiClientModel)CurrentPlayersList[0]).ClientPlayer.IdType;

            // Place the starting pieces
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2 - 1, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[0]).ClientPlayer));
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2 - 1, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[0]).ClientPlayer));

            // MessageBox.Show(Gameboard.DrawGameboard());

            // Set the First player to be the current player
            //            CurrentPlayer = CurrentPlayersList[0].ClientPlayer.PlayerId;
        }


        /// <summary>
        /// The main game routine
        /// </summary>
        public override void StartGame()
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
            //Gameboard.AddPiece(18, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            //Gameboard.AddPiece(17, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            //Gameboard.AddPiece(19, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            //Gameboard.AddPiece(21, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            //Gameboard.AddPiece(12, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            //MessageBox.Show(Gameboard.DrawGameboard());

            GameHasStarted = true;

        }

        /// <summary>
        /// Checks if there is a valid move for the specified player.
        /// </summary>
        /// <returns></returns>
        public override bool CheckGameOver(Players player)
        {
            bool GameIsOver = true;

            for (int i = 0; i < Gameboard.Rows * Gameboard.Cols; i++)
            {
                if (ValidateTilePlacement(i, player))
                {
                    GameIsOver = false;
                    AvailableMovesList.Add(i);
                }
            }

            return GameIsOver;
        }

        public override bool PlayTurn()
        {
            // Clear the tiles to turn array
            AvailableMovesList.Clear();
            TilesToTurn.Clear();

            return (MakePlayerMove(CurrentMoveIndex));
        }


        #endregion

        #region Public Methods
        public string DisplayGamePlayers()
        {
            string str = string.Empty;
            str += "----- PLAYER INFO -----\n";
            foreach (KeyValuePair<int, ClientModel> item in CurrentPlayersList)
            {
                ReversiClientModel model = (ReversiClientModel) item.Value;
                str += model.ClientPlayer.PlayerId + "   " + model.ClientPlayer.Name + "    " +
                       model.ClientPlayer.IdType + "\n";
            }

            return str;
        }

        /// <summary>
        /// Make a list of the current player sockets for the game.
        /// </summary>
        /// <returns></returns>
        public List<TcpClient> GetPlayersSocketList()
        {
            List<TcpClient> list = new List<TcpClient>();

            foreach (KeyValuePair<int,ClientModel> item in CurrentPlayersList)
            {
                ReversiClientModel model = (ReversiClientModel)item.Value;
                list.Add(model.ConnectionSocket);
            }

            return list;
        }

        public override void RemovePlayerFromGame(ClientModel model)
        {

            base.RemovePlayerFromGame(model);
        }

        public override string DisplayGameInfoComplete()
        {
            string str = string.Empty;
            str += "----- GAME INFO -----\n";
            str += DisplayGameStats() + "\n";
            str += DisplayGamePlayers() + "\n";
            str += Gameboard.DisplayGameboardStats() + "\n";

            return str;
        }
        #endregion

    }
}