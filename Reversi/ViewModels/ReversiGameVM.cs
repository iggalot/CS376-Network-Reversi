using Reversi.Models;
using Reversi.ViewModels;

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
