using System;
using System.Collections.Generic;
using ClientServerLibrary;

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


    [Serializable]
    public class ChatModel : ClientServerInfoModel
    {

        #region Public Properties
        /// <summary>
        /// The game ID for this game
        /// </summary>
        public int ChatId { get; private set; }

        /// <summary>
        /// A list of the players in the chat session
        /// </summary>
        public PlayerModel[] CurrentPlayersList { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for a game object
        /// </summary>
        /// <param name="list">List of players to join the game</param>
        public ChatModel(List<PlayerModel> list)
        {
            // Set our game id
            ChatId = NextId();

            // Initialize our players list
            CurrentPlayersList = new PlayerModel[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                // create our empty players
                CurrentPlayersList[i] = list[i];
            }
        }

        #endregion

    }
}
