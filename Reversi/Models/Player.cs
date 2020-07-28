using System;
using System.Net.Sockets;

namespace Reversi.Models
{
    /// <summary>
    /// A class that defines the basic game player
    /// </summary>
    
    [Serializable]
    public class Player
    {
        /// <summary>
        /// The ID of our player
        /// </summary>
        public Players ID { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The socket for this players connection
        /// /summary>

        [NonSerialized()] public TcpClient Socket;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Player()
        {
            ID = Players.UNDEFINED;
            Name = "Some bloke...";
            Socket = null;
        }

        /// <summary>
        /// Constructor for a new player
        /// </summary>
        /// <param name="id">The player number</param>
        /// <param name="name">The name of the player</param>
        /// <param name="socket">The associated socket for this player</param>
        public Player(Players id, string name, TcpClient socket)
        {
            ID = id;
            Name = name;
            Socket = socket;
        }
    }
}
