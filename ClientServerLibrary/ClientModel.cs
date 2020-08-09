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
        /// <param name="listener">The server listener</param>
        public ClientModel(TcpClient client, TcpListener listener) : base(client, listener)
        {
            // Store the client process
            ClientProcess = Process.GetCurrentProcess();
            ClientMainThread = Thread.CurrentThread;
        }

        /// <summary>
        /// A basic copy constructor of a clientModel
        /// </summary>
        /// <param name="clientModel"></param>
        public ClientModel(ClientModel clientModel) : base()
        {
            ClientProcess = clientModel.ClientProcess;
            ClientMainThread = clientModel.ClientMainThread;
            Id = clientModel.Id;
            CurrentStatus = clientModel.CurrentStatus;
            ShouldShutdown = clientModel.ShouldShutdown;

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Function to establish a connection between this client and the game server
        /// that returns the status of the connection
        /// </summary>
        /// <param name="port">Server port</param>
        /// <param name="statusMessage">Status message of the connection attempt</param>
        /// <param name="address">Server address</param>
        /// <returns></returns>
        public bool ConnectClient(string address, Int32 port, out string statusMessage)
        {
            // Otherwise try to make the connection
            try
            {
                // save the socket once the connection is made
                ConnectionSocket = MakeConnection(address, port);
                ConnectionSocket.GetStream();
            }
            catch (ArgumentNullException e)
            {
                statusMessage = "ArgumentNullException: {0}";
                Console.WriteLine(statusMessage, e);
                return false;
            }
            catch (SocketException e)
            {
                statusMessage = "SocketException: {0}";
                Console.WriteLine(statusMessage, e);
                return false;
            }
            catch
            {
                statusMessage = "Unable to connect to socket..";
                Console.WriteLine(statusMessage);
                return false;
            }

            statusMessage = "Connected to server...";
            return true;
        }
        #endregion


    }
}

