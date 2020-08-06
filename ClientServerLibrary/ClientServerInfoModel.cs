using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    [Serializable]
    public class ClientServerInfoModel : DataTransmission, IConnectionHandler
    {
        private static int nextId = 0;
        
        /// <summary>
        /// The current status for this model object
        /// </summary>
        public ConnectionStatusTypes CurrentStatus { get; set; }

        /// <summary>
        /// The id for this client
        /// </summary>
        public int ID { get; private set; }

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

        public int NextID() 
        {
            nextId++;
            return nextId;
        }


        
        #region Constructor
        public ClientServerInfoModel(TcpClient connection, TcpListener listener)
        {
            ID = NextID();
            ConnectionSocket = connection;
            ListenerSocket = listener;
            CurrentStatus = ConnectionStatusTypes.STATUS_CONNECTION_UNKNOWN; 
        }
        #endregion

        #region IConnectionHandler Implementation

        public virtual void AcceptConnection(ClientModel model)
        {
            throw new System.NotImplementedException();
        }

        public virtual void RefuseConnection(ClientModel model)
        {
            throw new System.NotImplementedException();
        }

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

        public virtual void AcceptOrRefuseConnection(ClientModel model, out ConnectionStatusTypes status)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
