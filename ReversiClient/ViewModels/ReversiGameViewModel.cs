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

                if (value.GameboardModel == null)
                    return;
                
                _reversiGameboardViewModel = new ReversiGameboardViewModel(value.GameboardModel);

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

 


        #endregion


    }
}
