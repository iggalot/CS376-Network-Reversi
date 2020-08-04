using Reversi.Models;

namespace Reversi.ViewModels
{
    public class ReversiGamepieceVM
    {
        private GamePieceModel _reversiGamepiece;

        /// <summary>
        /// The model object for this view model
        /// </summary>
        public GamePieceModel Model {
            get => _reversiGamepiece;
            set
            {
                if (value == null)
                    return;
                _reversiGamepiece = value;
            }
        }

        public ReversiGamepieceVM(GamePieceModel piece)
        {
            Model = piece;
        }
    }
}
