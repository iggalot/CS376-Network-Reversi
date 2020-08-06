using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace ClientServerLibrary
{
    /// <summary>
    /// A generic client class utilizing the TcpClient protocol.  Requires a TcpListener 
    /// on the other end to create the socket before communications can start.
    /// </summary>

    [Serializable]
    public class ClientModel : ClientServerInfoModel
    {
        /// <summary>
        /// The listener thread for this client
        /// </summary>
        [NonSerialized()] public Thread ListenThread;

        /// <summary>
        /// The main application thread for this client
        /// </summary>
        [NonSerialized()] public Thread ClientMainThread;

        /// <summary>
        /// Stores the process id for this client.
        /// </summary>
        [NonSerialized()] public Process ClientProcess = null;



        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientModel() : base(null,null)
        {
            // Store the client process
            ClientProcess = Process.GetCurrentProcess();
            ClientMainThread = Thread.CurrentThread;
        }

        /// <summary>
        /// Creates a client model for a client socket
        /// </summary>
        /// <param name="client">The client socket</param>
        public ClientModel(TcpClient client, TcpListener listener) : base(client, listener)
        {
            // Store the client process
            ClientProcess = Process.GetCurrentProcess();
            ClientMainThread = Thread.CurrentThread;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Function to establish a connection between this client and the game server
        /// that returns the status of the connection
        /// </summary>
        /// <param name="status_message">Status message of the connection attempt</param>
        /// <returns></returns>
        public bool ConnectClient(string address, Int32 port, out string status_message)
        {
            NetworkStream serverStream;

            // Otherwise try to make the connection
            try
            {
                // save the socket once the connection is made
                ConnectionSocket = MakeConnection(address, port);
                serverStream = ConnectionSocket.GetStream();
            }
            catch (ArgumentNullException excep)
            {
                status_message = "ArgumentNullException: {0}";
                Console.WriteLine(status_message, excep);
                return false;
            }
            catch (SocketException excep)
            {
                status_message = "SocketException: {0}";
                Console.WriteLine(status_message, excep);
                return false;
            }
            catch
            {
                status_message = "Unable to connect to socket..";
                Console.WriteLine(status_message);
                return false;
            }

            status_message = "Connected to server...";
            return true;
        }
        #endregion



        #region IConnectionHandler Interface Implementation
        public override void AcceptConnection(ClientModel model)
        {
            // Do nothing since a client shouldnt have this functionality
        }
        public override void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public override TcpClient ListenForConnections()
        {
            throw new NotImplementedException();
        }

        public override void RefuseConnection(ClientModel model)
        {
            // Do nothing since a client shouldnt have this functionality
        }

        public override void AcceptOrRefuseConnection(ClientModel model, out ConnectionStatusTypes status)
        {
            // Do nothing since a client shouldnt have this functionality
            status = ConnectionStatusTypes.STATUS_CONNECTION_INCAPABLE;
        }
        #endregion

    }
}

