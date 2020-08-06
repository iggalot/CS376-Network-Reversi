using ClientServerLibrary;
using System;
using System.Net.Sockets;

namespace Reversi.Models
{
    [Serializable]
    public class ReversiClientModel : ClientModel
    {
        #region Public  Properties
        /// <summary>
        /// The game model associated with this client
        /// </summary>
        public ReversiGameModel Game { get; set; }

        /// <summary>
        /// The player model associated with this client
        /// </summary>
        public PlayerModel ClientPlayer { get; set; }

        /// <summary>
        /// Stores the object data for the last move sent to the server.
        /// </summary>
        public GameMoveModel LastMove { get; set; }


        #endregion

        #region Constructor
        /// <summary>
        /// A default constructor
        /// </summary>
        public ReversiClientModel() : base(null, null) { }

        /// <summary>
        /// The more formal connected client model structor
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="listenerSocket"></param>
        public ReversiClientModel(TcpClient clientSocket, TcpListener listenerSocket) : base(clientSocket, listenerSocket)
        {
            Game = null;
            LastMove = null;
            ClientPlayer = new PlayerModel();
        }

        public ReversiClientModel(ClientModel clientModel, string name) : base(clientModel)
        {
            Game = null;
            LastMove = null;
            ClientPlayer = new PlayerModel(clientModel.ID, Players.UNDEFINED, name);
            this.ClientProcess = clientModel.ClientProcess;
            this.ClientMainThread = clientModel.ClientMainThread;
        }

        #endregion

        #region Public Methods


        #endregion

    }
}
