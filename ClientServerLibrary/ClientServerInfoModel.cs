using System.Net.Sockets;

namespace ClientServerLibrary
{
    public class ClientServerInfoModel : DataTransmission
    {
        private static int nextId = 0;

        /// <summary>
        /// The id for this client
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The connection this object has to its server
        /// </summary>
        public TcpClient ConnectionSocket { get; set; }

        /// <summary>
        /// The TcpListener socket for this server instance
        /// </summary>
        public TcpListener ListenerSocket { get; set; } = null;

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
        }
        #endregion

    }
}
