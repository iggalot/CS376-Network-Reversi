using GameObjects.Models;
using System.Collections.ObjectModel;

namespace Reversi.ViewModels
{
    public class ReversiGameboardVM : BaseViewModel
    {
        private ObservableCollection<ReversiSquareVM> _reversiSquareCollection = new ObservableCollection<ReversiSquareVM>();
        private BoardModel _reversiGameboard;

        public static int NumRows { get; set; }
        public static int NumCols { get; set; }

        public string GameboardViewModelText { get; set; } = "My gameboard viewmodel text";


        /// <summary>
        /// The board model for this view model
        /// </summary>
        public BoardModel Model 
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
        public ReversiGameboardVM(BoardModel board)
        {
            if (board == null)
                return;

            Model = board;
            NumRows = board.Rows;
            NumCols = board.Cols;

            ReversiGameboardVMCollection = new ObservableCollection<ReversiSquareVM>();

            foreach(SquareModel square in board.Squares)
            {
                this.AddSquareVM(square);
            }
        }

        /// <summary>
        /// Creates a reversi square vm for a specified square model and adds it to the gameboard square collection
        /// </summary>
        /// <param name="s"></param>
        private void AddSquareVM(SquareModel s)
        {
            ReversiSquareVM squareVm = new ReversiSquareVM(s);
            ReversiGameboardVMCollection.Add(squareVm);
        }

        public static double CanvasLeftPosition(int index, int width)
        {
            int numRows = NumRows;
            int numCols = NumCols;

            int col = index % numRows;
            return (col * width);
        }

        public static double CanvasTopPosition(int index, int height)
        {
            int numRows = NumRows;
            int numCols = NumCols;

            int row = index / numCols;
            return (row * height);
        }

    }
}
