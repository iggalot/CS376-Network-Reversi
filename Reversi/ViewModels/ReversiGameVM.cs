using GameObjects.Models;
using Reversi.Models;

namespace Reversi.ViewModels
{
    public class ReversiGameVM : BaseViewModel
    {
        private ReversiGameboardVM _reversiGameboardVM;
        private ReversiGameModel _reversiGame;

        public ReversiGameboardVM GameboardVM { 
            get => _reversiGameboardVM;
            private set
            {
                if (value == null)
                    return;
                _reversiGameboardVM = value;

                OnPropertyChanged("GameboardVM");
                OnPropertyChanged("P1Score");
                OnPropertyChanged("P2Score");
            } 
        }

        public int LastMoveIndex
        {
            get => Model.CurrentMoveIndex;
            set
            {
                if (value == null)
                    return;

                Model.CurrentMoveIndex = value;

                OnPropertyChanged("LastMoveIndex");
            }
        }

        /// <summary>
        /// The game model object for this view model
        /// </summary>
        public ReversiGameModel Model {
            get => _reversiGame;
            set
            {
                if (value == null)
                    return;
                _reversiGame = value;
            } 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game model</param>
        public ReversiGameVM(ReversiGameModel game)
        {
            Model = game;

            if (game?.Gameboard == null)
                return;

            GameboardVM = new ReversiGameboardVM(game.Gameboard);
        }
    }
}
