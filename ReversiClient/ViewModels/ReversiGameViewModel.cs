using Reversi.Models;

namespace ReversiClient.ViewModels
{
    public class ReversiGameViewModel
    {
        #region Private Properties
        // The default dimensions of the squares on our gameboard.
        private readonly double squareWidth = 30;
        private readonly double squareHeight = 30;

        #endregion

        #region Public Properties

        /// <summary>
        /// The model associated with this view model
        /// </summary>
        public ReversiGame Model { get; set; }

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
        }

        #endregion


    }
}
