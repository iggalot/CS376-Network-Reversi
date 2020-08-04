using Reversi.Models;

namespace Reversi.ViewModels
{
    public class ReversiSquareVM
    {
        private ReversiSquareVM _reversiSquareVM;
        private ReversiGamepieceVM _reversiGamepieceVM;

        /// <summary>
        /// The view model for this square
        /// </summary>
        public ReversiSquareVM Instance
        {
            get => _reversiSquareVM;
            private set
            {
                if (value == null)
                    return;
                _reversiSquareVM = value;
            }
        }

        /// <summary>
        /// The view model for the piece contained by this square
        /// </summary>
        public ReversiGamepieceVM PieceVM
        {
            get => _reversiGamepieceVM;

            private set
            {
                if (value == null)
                    return;
                _reversiGamepieceVM = value;
            }
        }

        /// <summary>
        /// The model object behind this view model
        /// </summary>
        public SquareModel Model { get; set; }

        public ReversiSquareVM(SquareModel square)
        {
            if (square == null)
                return;

            Model = square;

            PieceVM = new ReversiGamepieceVM(square.Piece);
        }
    }
}
