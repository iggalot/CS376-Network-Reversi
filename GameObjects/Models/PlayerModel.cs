using System;

namespace GameObjects.Models
{
    /// <summary>
    /// A class that defines the basic game player
    /// </summary>

    [Serializable]
    public class PlayerModel
    {

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public int PlayerId { get; set; } = -1;

        /// <summary>
        /// The IDType of our player as a player type
        /// </summary>
        public Players IdType { get; set; }


        /// <summary>
        /// The name of the player
        /// </summary>
        public string Name { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerModel()
        {
            PlayerId = -1;
            IdType = Players.Undefined;
            Name = "Some bloke...";
        }

        /// <summary>
        /// Constructor for a new player
        /// </summary>
        /// <param name="id">The player number</param>
        /// <param name="name">The name of the player</param>
        /// <param name="idType">The player type for this player</param>
        public PlayerModel(int id, Players idType, string name)
        {
            PlayerId = id;
            IdType = idType;
            Name = name;
        }

        public string DisplayPlayerInfo()
        {
            string str = string.Empty;
            return (PlayerId + "  " + Name + " --- " + IdType);
        }
    }
}
