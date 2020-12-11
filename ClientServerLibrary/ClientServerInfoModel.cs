using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    [Serializable]
    public class ClientServerInfoModel : DataTransmission, IConnectionHandler
    {
        private static int _nextId = 0;
        
        /// <summary>
        /// The current status for this model object
        /// </summary>
        public ConnectionStatusTypes CurrentStatus { get; set; }

        /// <summary>
        /// Thhe time at which this client was created.
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// The id for this client
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The connection this object has to its server
        /// </summary>
        [NonSerialized()] public TcpClient ConnectionSocket = null;

        /// <summary>
        /// The TcpListener socket for this server instance
        /// </summary>
        [NonSerialized()] public TcpListener ListenerSocket = null;

        /// <summary>
        /// Tells the server / client it should shutdown
        /// </summary>
        public bool ShouldShutdown { get; set; } = false;

        public int NextId() 
        {
            _nextId++;
            return _nextId;
        }

        public string ListInfo()
        {
            return (Id + " : " + CurrentStatus + " : " + ShouldShutdown);
        }



        #region Constructor
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public ClientServerInfoModel()
        {
            Id = -1;
            ConnectionSocket = null;
            ListenerSocket = null;
            CurrentStatus = ConnectionStatusTypes.StatusConnectionUnknown;
            TimeCreated = DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="listener"></param>
        public ClientServerInfoModel(TcpClient connection, TcpListener listener)
        {
            Id = NextId();
            ConnectionSocket = connection;
            ListenerSocket = listener;
            CurrentStatus = ConnectionStatusTypes.StatusConnectionUnknown;
            TimeCreated = DateTime.Now;
        }
        #endregion

        #region IConnectionHandler Implementation

        /// <summary>
        /// Connects our client to a TcpIp socket
        /// </summary>
        /// <param name="v">Address to connect to</param>
        /// <param name="port">Port number to connect to</param>
        /// <returns></returns>
        public TcpClient MakeConnection(string v, Int32 port)
        {
            return new TcpClient(v, port);
        }

        public virtual void CloseConnection()
        {
            throw new System.NotImplementedException();
        }

        public virtual TcpClient ListenForConnections()
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
