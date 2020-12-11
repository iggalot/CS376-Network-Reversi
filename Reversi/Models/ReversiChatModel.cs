using System;
using System.Collections.Generic;
using GameObjects.Models;

namespace Reversi.Models
{
    [Serializable]
    public class ReversiChatModel : ChatModel
    {
        #region Public Properties
        /// <summary>
        /// An array to hold the indices of tiles to turn in a given round.
        /// </summary>
        /// 
        public string TestChatText { get; set; } = "My test game text";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for our Reversi game
        /// </summary>
        /// <param name="list">The list of participating player objects</param>
        public ReversiChatModel(List<PlayerModel> list) : base(list)
        {
            // Setup the chat session
            SetupChat();
        }
        #endregion

        #region Public Methods

        public void SetupChat()
        {

        }

        #endregion
    }
}