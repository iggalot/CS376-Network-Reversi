using Reversi.Models;

namespace ReversiClient.ViewModels
{
    public class ReversiGameViewModel : BaseViewModel
    {
        #region Private Properties
        // The default dimensions of the squares on our gameboard.
        private readonly double squareWidth = 30;
        private readonly double squareHeight = 30;

        private ReversiGameboardViewModel _reversiGameboardViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The model associated with this view model
        /// </summary>
        public ReversiGame Model { get; set; }

        /// <summary>
        /// The related gameboard view model to this game view model
        /// </summary>
        public ReversiGameboardViewModel GameboardViewModel { 
            get => _reversiGameboardViewModel; 
            set
            {
                if (value == null)
                    return;

                if (value.Model == null)
                    return;
                
                _reversiGameboardViewModel = new ReversiGameboardViewModel(value.Model);

                OnPropertyChanged("GameboardViewModel");
            } 
        }

        /// <summary>
        /// A test string
        /// </summary>
        public string TestText { get; set; } = "24";

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">The game model for this view model</param>
        public ReversiGameViewModel(ReversiGame game)
        {
            Model = game;

            if (game == null)
                return;

            if (game.Gameboard == null)
                return;

            GameboardViewModel = new ReversiGameboardViewModel(game.Gameboard);
        }

        #endregion

        #region Public Methods

        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0; // upper left x-coordinate of game square pixel
            int nextY = 0; // upper left y-coordinate of game square pixel
            int index = 0;
            int rowCounter = 0;
            int colCounter;
            bool nextIsOdd = false;

            // Our gamepieces
            //double pieceWidth = (gameboardVM.GameboardVM[index].SquareWidth * 0.8);
            //double pieceHeight = (gameboardVM.GameboardVM[index].SquareHeight * 0.8);

            //// Compute the offset to center the piece in the game square
            //double xOffset = ((int)gameboardVM.GameboardVM[index].SquareWidth - pieceWidth) / 2.0;
            //double yOffset = ((int)gameboardVM.GameboardVM[index].SquareHeight - pieceHeight) / 2.0;


            //while (!doneDrawingBackground)
            //{
            //    // Our game square
            //    Rectangle rect = new Rectangle
            //    {
            //        Width = (int)gameboardVM.GameboardVM[index].SquareWidth,
            //        Height = (int)gameboardVM.GameboardVM[index].SquareHeight,
            //        Fill = Brushes.DarkGreen,
            //        StrokeThickness = 1,
            //        Stroke = Brushes.Black
            //    };

            //    GameArea.Children.Add(rect);
            //    Canvas.SetTop(rect, nextY);
            //    Canvas.SetLeft(rect, nextX);

            //    // Create our game pieces for the board based on the gameboard model
            //    Gamepiece piece = new Gamepiece(gameboard.GameBoard[index].Piece.Owner, gameboard.GameBoard[index].Piece.PieceShape);
            //    GamepieceViewModel pieceVM = new GamepieceViewModel(gameboardVM.GameboardVM[index].PieceVM.Piece, pieceWidth, pieceHeight);
            //    GameArea.Children.Add(pieceVM.PieceShape);
            //    Canvas.SetTop(pieceVM.PieceShape, nextY + yOffset);
            //    Canvas.SetLeft(pieceVM.PieceShape, nextX + xOffset);

            //    // if there's not a game piece in the square, add the click and hover events
            //    if (piece.Owner == Tokens.TokenUnclaimed)
            //    {
            //        // Add a mouse click event to the game square
            //        rect.MouseUp += new MouseButtonEventHandler(square_MouseUp);

            //        // Add a mouse hover on enter / leave event
            //        rect.MouseEnter += new MouseEventHandler(shape_MouseEnter);
            //        rect.MouseLeave += new MouseEventHandler(shape_MouseLeave);
            //    }

            //    colCounter = index % gameboard.rows;

            //    nextIsOdd = !nextIsOdd;
            //    nextX += (int)gameboardVM.GameboardVM[index].SquareWidth;

            //    // Make the top and bottom border rows blue
            //    if ((rowCounter == 0) || (rowCounter == gameboard.rows - 1))
            //    {
            //        rect.Fill = Brushes.Blue;
            //    }

            //    // Make the left and right border cols red
            //    if ((colCounter % gameboard.cols == 0) || (colCounter % gameboard.cols == (gameboard.cols - 1)))
            //    {
            //        rect.Fill = Brushes.Red;
            //    }

            //    // If we've reached the end of the current
            //    if (colCounter == gameboard.cols - 1)
            //    {
            //        nextX = 0;
            //        nextY += (int)gameboardVM.GameboardVM[index].SquareHeight;
            //        rowCounter++;
            //        nextIsOdd = (rowCounter % 2 != 0);
            //    }

            //    index++;

            //    // If we've reached the last element of the gameboard, then set the flag for done
            //    if ((rowCounter >= gameboard.rows) || (index >= gameboardVM.GameboardVM.Count))
            //        doneDrawingBackground = true;
            //}
        }



        #endregion


    }
}
