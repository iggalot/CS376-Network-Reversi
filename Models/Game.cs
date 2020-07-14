using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;

namespace Reversi.Models
{
    public class Game
    {
        #region Public Properties
        /// <summary>
        /// Flag to determine if the game is over
        /// </summary>
        public bool IsGameOver { get; set; } = false;

        /// <summary>
        /// The current player whos turn is active
        /// </summary>
        public Player CurrentPlayer { get; set; }

        /// <summary>
        /// The opponent to the current player
        /// </summary>
        public Player CurrentOpponent { get; set; }

        /// <summary>
        /// The index of the current move
        /// </summary>
        public int CurrentMoveIndex { get; set; } = 20;


        /// <summary>
        /// A list of the players for our game
        /// </summary>
        private Player[] Players { get; set; }

        /// <summary>
        /// The gameboard for our game
        /// </summary>
        public Board Gameboard { get; set; }

        /// <summary>
        /// An array to hold the indices of tiles to turn in a given round.
        /// </summary>
        public List<int> TilesToTurn { get; set; } = new List<int>();
        #endregion

        #region Constructor
        public Game(int num_players)
        {
            // create our gameboard
            Gameboard = new Board(8, 8);

            // Initialize our players list
            Players = new Player[num_players];

            // create our players
            Players[0] = new Player(Models.Players.PLAYER1, "Bob");
            Players[1] = new Player(Models.Players.PLAYER2, "Alice");
        }

        internal void SetupGame()
        {
            // TODO:  What if the board has odd rows and columns ODD numbered?
            var midpoint = Gameboard.Rows * Gameboard.Cols / 2;

            // Place the starting pieces
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2 - 1, new GamePiece(Pieceshapes.ELLIPSE, Players[0]));
            Gameboard.AddPiece(midpoint - Gameboard.Cols / 2, new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2 -1 , new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            Gameboard.AddPiece(midpoint + Gameboard.Cols / 2, new GamePiece(Pieceshapes.ELLIPSE, Players[0]));

            MessageBox.Show(Gameboard.DrawGameboard());

            // Set the First player to be the current player
            CurrentPlayer = Players[0];
            CurrentOpponent = Players[1];
        }
        #endregion

        /// <summary>
        /// Toggle the current and opponent player properties for the game
        /// </summary>
        public void NextPlayer()
        {
            CurrentOpponent = CurrentPlayer;
            CurrentPlayer = (CurrentPlayer == Players[0]) ? Players[1] : Players[0];
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
            Gameboard.AddPiece(18, new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            Gameboard.AddPiece(17, new GamePiece(Pieceshapes.ELLIPSE, Players[0]));
            Gameboard.AddPiece(19, new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            Gameboard.AddPiece(21, new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            Gameboard.AddPiece(12, new GamePiece(Pieceshapes.ELLIPSE, Players[1]));
            MessageBox.Show(Gameboard.DrawGameboard());

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
            for(int i=0; i < Gameboard.Rows * Gameboard.Cols; i++)
            {
                if(!ValidatePlacement(i))
                {
                    return false;
                }
            }

            return true;
        }

        internal void PlayRound()
        {
            // Make a move for player 1
            MakePlayerMove(CurrentMoveIndex);
            NextPlayer();
        }

        /// <summary>
        /// Primary routine for a player to make a move
        /// </summary>
        /// <param name="index">the index of the move being made</param>
        internal void MakePlayerMove(int index)
        {
            Player player = CurrentPlayer;

            if(ValidatePlacement(index))
            {
                // Add a new game piece at the location
                GamePiece piece = new GamePiece(Pieceshapes.ELLIPSE, player);
                Gameboard.AddPiece(index, piece);

                // Capture the opponents tiles
                DoTurnTiles();

                // Reset the tiles to be turned array
                TilesToTurn.Clear();
            }
            MessageBox.Show(Gameboard.DrawGameboard() + "\nCurrent Player: " + CurrentPlayer.ID + " : " + CurrentPlayer.Name);
        }

        /// <summary>
        /// Routine that changes the owner of a tile based on indices stored in TilesToTurn list.
        /// </summary>
        private void DoTurnTiles()
        {
            Player player = CurrentPlayer;

            foreach (int index in TilesToTurn)
            {
                Gameboard.Squares[index].Piece.Owner = player;

                // For each tile being flipped...play a sounds
                PlaySounds(GameSounds.SOUND_FLIPTILE);


            }

            // Now clear the tiles to turn array since all the moves have been made
            TilesToTurn.Clear();
        }

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
            Player player = CurrentPlayer;
            Player opponent = (player == Players[0]) ? Players[1] : Players[0];

            // Make sure that we are within acceptable index ranges, otherwise return
            if(index < 0 || index >= Gameboard.Squares.Length)
            {
                return false;
            }
            // Selected index must not already contain a piece
            if(Gameboard.Squares[index].Piece != null)
            {
                //MessageBox.Show("This block already contains a piece.");
                return false;
            }

            var tmp_index = index;
            // 1 2 3
            // 4 X 5
            // 6 7 8
            // A neightbor must be owned by the opposite player
            foreach(DirectionVectors dv in Enum.GetValues(typeof(DirectionVectors)))
            {
                isValidMove = false;

                tmp_index = Gameboard.GetIndexByOffsets(index, dv);

                // Is there a valid piece in this square?
                if ((tmp_index == -1) || (Gameboard.Squares[tmp_index].Piece == null))
                    continue;

                // Check if the neighbor is owned by the opponent
                Player owner = Gameboard.Squares[tmp_index].Piece.Owner;

                if (owner != opponent)
                {
                    // Neighbor isn't the opponent so not a valid move
                    continue;
                } else
                {
                    // Store indices in a list while we search
                    List<int> tmpList = new List<int>();

                    // Otherwise continue searching in this direction to see if a 
                    // players piece is also in this direction.
                    int next_index = tmp_index;
                    Player next_owner = owner;

                    // Continue searching so long as we don't reach the border (-1) and the next square is 
                    // owned by the opponent.
                    while((next_index != -1) && (next_owner == opponent))
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
                        if (next_owner == player)
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

        internal void PlaySounds(GameSounds sound)
        {
            string soundString="";

            // chimes.wav
            // Windows Default.wav -- click to place
            // Windows User Account Control.wav -- click to place
            // tada.wav -- successful move
            switch (sound)
            {
                case GameSounds.SOUND_CLICK_SUCCESSFUL:
                    soundString = @"c:\Windows\Media\Windows User Account Control.wav";
                    break;
                case GameSounds.SOUND_FLIPTILE:
                    soundString = @"c:\Windows\Media\chimes.wav";
                    break;
                case GameSounds.SOUND_TURN_COMPLETE:
                    soundString = @"c:\Windows\Media\tada.wav";
                    break;
                default:
                    return;
            }
            using (var soundPlayer = new SoundPlayer(soundString))
            {
                Thread.Sleep(500);
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
        }
    }
}
