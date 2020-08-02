using Reversi.Models;

namespace Reversi.ViewModels
{
    public class ReversiGamepieceVM
    {
        private GamePiece _reversiGamepiece;

        /// <summary>
        /// The model object for this view model
        /// </summary>
        public GamePiece Model {
            get => _reversiGamepiece;
            set
            {
                if (value == null)
                    return;
                _reversiGamepiece = value;
            }
        }

        public ReversiGamepieceVM(GamePiece piece)
        {
            Model = piece;
        }
    }
}
