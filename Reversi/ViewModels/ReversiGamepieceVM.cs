using GameObjects.Models;

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

        public static int Width { get; set; } = 30;
        public static int Height { get; set; } = 30;

        public ReversiGamepieceVM(GamePieceModel piece)
        {
            Model = piece;
        }
    }
}
