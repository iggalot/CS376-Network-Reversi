using Reversi.Models;
using System.Collections.ObjectModel;

namespace Reversi.ViewModels
{
    public class ReversiGameboardVM : BaseViewModel
    {
        private ObservableCollection<ReversiSquareVM> _reversiSquareCollection = new ObservableCollection<ReversiSquareVM>();
        private Board _reversiGameboard;

        public static int GameboardSquareWidth { get; set; } = 50;
        public static int GameboardSquareHeight { get; set; } = 50;

        public static int NumRows { get; set; }
        public static int NumCols { get; set; }

        public string GameboardViewModelText { get; set; } = "My gameboard viewmodel text";


        /// <summary>
        /// The board model for this view model
        /// </summary>
        public Board Model 
        {
            get => _reversiGameboard;
            private set
            {
                if (value == null)
                    return;
                _reversiGameboard = value;

                OnPropertyChanged("Model");
            }

        }

        /// <summary>
        /// The collection of game square view models that make up this gameboard
        /// </summary>
        public ObservableCollection<ReversiSquareVM> ReversiGameboardVMCollection
        {
            get => _reversiSquareCollection;
            private set
            {
                if (value == null)
                    return;
                _reversiSquareCollection = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="board"></param>
        public ReversiGameboardVM(Board board)
        {
            if (board == null)
                return;

            Model = board;
            NumRows = board.Rows;
            NumCols = board.Cols;

            ReversiGameboardVMCollection = new ObservableCollection<ReversiSquareVM>();

            foreach(Square square in board.Squares)
            {
                this.AddSquareVM(square);
            }
        }

        /// <summary>
        /// Creates a reversi square vm for a specified square model and adds it to the gameboard square collection
        /// </summary>
        /// <param name="s"></param>
        private void AddSquareVM(Square s)
        {
            ReversiSquareVM square_vm = new ReversiSquareVM(s);
            ReversiGameboardVMCollection.Add(square_vm);
        }

        public static double CanvasLeftPosition(int index, int width)
        {
            int num_rows = NumRows;
            int num_cols = NumCols;

            int col = index % num_rows;
            return (col * width);
        }

        public static double CanvasTopPosition(int index, int height)
        {
            int num_rows = NumRows;
            int num_cols = NumCols;

            int row = index / num_cols;
            return (row * height);
        }

    }
}
