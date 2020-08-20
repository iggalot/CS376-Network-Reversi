using ClientServerLibrary;
using System;
using System.Net.Sockets;
using GameObjects.Models;

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

        /// <summary>
        /// The time when a player joined the wait list
        /// </summary>
        public DateTime TimeConnectedWhen { get; set; }

        /// <summary>
        ///  The time a player has been waiting
        /// </summary>
        public TimeSpan TimeWaiting => TimeConnectedWhen - DateTime.Now;

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
            TimeConnectedWhen = DateTime.Now;
        }

        public ReversiClientModel(ClientModel clientModel, string name) : base(clientModel)
        {
            ReversiClientModel model = (ReversiClientModel) clientModel;
            Game = null;
            LastMove = null;
            ClientPlayer = new PlayerModel(model.ClientPlayer.PlayerId, model.ClientPlayer.IdType, name);

            this.ClientProcess = clientModel.ClientProcess;
            this.ClientMainThread = clientModel.ClientMainThread;
            this.TimeConnectedWhen = DateTime.Now;
        }

        #endregion
    }
}
