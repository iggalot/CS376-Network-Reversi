using Reversi.Models;
using System.ComponentModel;

namespace ReversiClient.ViewModels
{
    public class ReversiGameboardViewModel : BaseViewModel
    {
        private Board _gameboardModel;

        public static int GameboardSquareWidth { get; set; } = 50;
        public static int GameboardSquareHeight { get; set; } = 50;

        public static int NumRows { get; set; }
        public static int NumCols { get; set; }

        public string GameboardViewModelText { get; set; } = "My gameboard viewmodel text";
        public Board GameboardModel {
            get => _gameboardModel;
            set
            {
                if (value == null)
                    return;
                _gameboardModel = value;

                OnPropertyChanged("GameboardModel");
            }
        }
        public ReversiGameboardViewModel(Board gameboard)
        {
            NumRows = gameboard.Rows;
            NumCols = gameboard.Cols;
            GameboardModel = gameboard;
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
