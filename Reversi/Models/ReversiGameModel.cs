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
        /// Has the P1 player gone yet?
        /// </summary>
        public bool P1Went { get; set; } = false;

        /// <summary>
        /// Has the P2 player gone yet?
        /// </summary>
        public bool P2Went { get; set; } = false;
        #endregion

        #region Constructors

        ///// <summary>
        ///// Constructor for our Reversi game
        ///// </summary>
        ///// <param name="list">The list of participating player objects</param>
        //public ReversiGameModel(List<PlayerModel> list) : base(list)
        //{
        //    // Setup the gameboard
        //    SetupGame();

        //    // Start the game
        //    StartGame();
        //}


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
            CurrentPlayer = (CurrentPlayer++) % ReversiSettings.ReversiPlayersPerGame;
        }

        /// <summary>
        /// Determine if the new placement location is valid.
        /// </summary>
        /// <param name="index">The square index where they are placing the piece</param>
        /// <returns></returns>
        public override bool ValidatePlacement(int index)
        {
            bool isValidMove = false;

            //Console.WriteLine("Index: " + index);

            //// Determine our opponent
            //int player = CurrentPlayer;
            //int opponent = (player == CurrentPlayersList[0].PlayerId) ? CurrentPlayersList[1].PlayerId : CurrentPlayersList[0].PlayerId;

            //// Make sure that we are within acceptable index ranges, otherwise return
            //if (index < 0 || index >= Gameboard.Squares.Length)
            //{
            //    return false;
            //}
            //// Selected index must not already contain a piece
            //if (Gameboard.Squares[index].Piece != null)
            //{
            //    //MessageBox.Show("This block already contains a piece.");
            //    return false;
            //}

            //var tmp_index = index;
            //// 1 2 3
            //// 4 X 5
            //// 6 7 8
            //// A neightbor must be owned by the opposite player
            //foreach (DirectionVectors dv in Enum.GetValues(typeof(DirectionVectors)))
            //{
            //    isValidMove = false;

            //    tmp_index = Gameboard.GetIndexByOffsets(index, dv);

            //    // Is there a valid piece in this square?
            //    if ((tmp_index == -1) || (Gameboard.Squares[tmp_index].Piece == null))
            //        continue;

            //    // Check if the neighbor is owned by the opponent
            //    PlayerModel owner = Gameboard.Squares[tmp_index].Piece.Owner;

            //    if (owner.PlayerId != opponent)
            //    {
            //        // Neighbor isn't the opponent so not a valid move
            //        continue;
            //    }
            //    else
            //    {
            //        // Store indices in a list while we search
            //        List<int> tmpList = new List<int>();

            //        // Otherwise continue searching in this direction to see if a 
            //        // players piece is also in this direction.
            //        int nextIndex = tmp_index;
            //        PlayerModel nextOwner = owner;

            //        // Continue searching so long as we don't reach the border (-1) and the next square is 
            //        // owned by the opponent.
            //        while ((nextIndex != -1) && (nextOwner.PlayerId == opponent))
            //        {
            //            // Add our element to the list.
            //            tmpList.Add(nextIndex);

            //            Console.WriteLine("...searching " + nextIndex + " to " + dv);

            //            nextIndex = Gameboard.GetIndexByOffsets(nextIndex, dv);


            //            // Did we find the border? Is there a valid piece in this square? 
            //            // If not, stop searching
            //            if ((nextIndex == -1) || (Gameboard.Squares[nextIndex].Piece == null))
            //            {
            //                tmpList.Clear(); // clear the temp list
            //                break;
            //            }

            //            nextOwner = Gameboard.Squares[nextIndex].Piece.Owner;

            //            // If neighbor to the neighbor in this direction is the same as the player,
            //            // the move is valid.
            //            if (nextOwner.PlayerId == player)
            //            {
            //                isValidMove = true;
            //                break;
            //            }
            //        }
            //        if (isValidMove)
            //        {
            //            Console.WriteLine("...VALID to " + dv);
            //            foreach (int item in tmpList)
            //                TilesToTurn.Add(item);
            //            tmpList.Clear();
            //        }
            //    }
            //}

            return isValidMove;
        }

        /// <summary>
        /// Primary routine for a player to make a move
        /// </summary>
        /// <param name="index">the index of the move being made</param>
        internal bool MakePlayerMove(int index)
        {
            //    PlayerModel player = null; ;
            //    bool playerFound = false;            

            //    foreach(PlayerModel item in CurrentPlayersList)
            //    {
            //        if (item.PlayerId == CurrentPlayer)
            //        {
            //            player = item;
            //            playerFound = true;
            //        }                    
            //    }

            //    if (player == null)
            //        return false;

            //    if (playerFound && ValidatePlacement(index))
            //    {


            //        // Add a new game piece at the location
            //        GamePieceModel piece = new GamePieceModel(Pieceshapes.Ellipse, player);
            //        Gameboard.AddPiece(index, piece);

            //        // Capture the opponents tiles
            //        DoTurnTiles();

            //        // Reset the tiles to be turned array
            //        TilesToTurn.Clear();
            //    }
        
            return true;
            //MessageBox.Show(Gameboard.DrawGameboard() + "\nCurrent Player: " + CurrentPlayer.IDType + " : " + CurrentPlayer.Name);
        }

        /// <summary>
        /// Routine that changes the owner of a tile based on indices stored in TilesToTurn list.
        /// </summary>
        private void DoTurnTiles()
        {
            //PlayerModel player = null; ;
            //bool playerFound = false;

            //foreach (ReversiClientModel item in CurrentPlayersList)
            //{
            //    if (item.ClientPlayer.PlayerId == CurrentPlayer)
            //    {
            //        player = item;
            //        playerFound = true;
            //    }
            //}

            //// If we didnt find the player, return
            //if (!playerFound)
            //    return;

            //foreach (int index in TilesToTurn)
            //{
            //    Gameboard.Squares[index].Piece.Owner = player;

            //    //// TODO: For each tile being flipped...play a sounds
            //    //ReversiSounds.PlaySounds(GameSounds.SOUND_FLIPTILE);
            //}

            // Now clear the tiles to turn array since all the moves have been made
            TilesToTurn.Clear();
        }

        public override void SetupGame()
        {
            // TODO:  What if the board has odd rows and columns ODD numbered?
            var midpoint = Gameboard.Rows * Gameboard.Cols / 2;

//            PlayerModel player1 = ((ReversiClientModel)CurrentPlayersList[0]).ClientPlayer.IdType;
            
            // Place the starting pieces
            //Gameboard.AddPiece(midpoint - Gameboard.Cols / 2 - 1, new GamePieceModel(Pieceshapes.Ellipse, CurrentPlayersList[0]));
            //Gameboard.AddPiece(midpoint - Gameboard.Cols / 2, new GamePieceModel(Pieceshapes.Ellipse, CurrentPlayersList[1]));
            //Gameboard.AddPiece(midpoint + Gameboard.Cols / 2 - 1, new GamePieceModel(Pieceshapes.Ellipse, CurrentPlayersList[1]));
            //Gameboard.AddPiece(midpoint + Gameboard.Cols / 2, new GamePieceModel(Pieceshapes.Ellipse, CurrentPlayersList[0]));

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
            Gameboard.AddPiece(18, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            Gameboard.AddPiece(17, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[0]).ClientPlayer));
            Gameboard.AddPiece(19, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            Gameboard.AddPiece(21, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
            Gameboard.AddPiece(12, new GamePieceModel(Pieceshapes.Ellipse, ((ReversiClientModel)CurrentPlayersList[1]).ClientPlayer));
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

            GameHasStarted = true;

        }

        /// <summary>
        /// Checks if there is a valid move for the current player.
        /// </summary>
        /// <returns></returns>
        public override bool CheckGameOver()
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

        public override bool PlayTurn()
        {
            return (MakePlayerMove(CurrentMoveIndex));
        }


        #endregion

        #region Public Methods
        //public string DisplayGamePlayers()
        //{
        //    string str = string.Empty;
        //    str += " ---------------------------------------------------\n";
        //    foreach (var player in CurrentPlayersList)
        //    {

        //        str += player.DisplayPlayerInfo() + "\n";
        //    }
        //    str += " ---------------------------------------------------\n";

        //    return str;
        //}

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
        #endregion

    }
}