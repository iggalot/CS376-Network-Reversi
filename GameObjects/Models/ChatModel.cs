using System;
using System.Collections.Generic;
using ClientServerLibrary;

namespace GameObjects.Models
{
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
