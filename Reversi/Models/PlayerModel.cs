using System;
using System.Net.Sockets;

namespace Reversi.Models
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
        public int PlayerID { get; set; } = -1;

        /// <summary>
        /// The IDType of our player as a player type
        /// </summary>
        public Players IDType { get; set; }


        /// <summary>
        /// The name of the player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The socket for this players connection
        /// /summary>

        [NonSerialized()] public TcpClient Socket;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerModel()
        {
            PlayerID = -1;
            IDType = Players.UNDEFINED;
            Name = "Some bloke...";
            Socket = null;
        }

        /// <summary>
        /// Constructor for a new player
        /// </summary>
        /// <param name="id">The player number</param>
        /// <param name="name">The name of the player</param>
        /// <param name="socket">The associated socket for this player</param>
        public PlayerModel(int num, Players id, string name, TcpClient socket)
        {
            PlayerID = num;
            IDType = id;
            Name = name;
            Socket = socket;
        }
    }
}
