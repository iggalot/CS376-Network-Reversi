using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Models.ReversiClient
{
    public class ReversiClientModel
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
        /// Flag to indicate if the client should shutdown
        /// </summary>
        public bool ClientShouldShutdown = false;

        /// <summary>
        /// Stores the object data for the last move sent to the server.
        /// </summary>
        public GameMoveModel LastMove { get; set; }

        /// <summary>
        /// Returns the socket connection to the game server
        /// </summary>
        public TcpClient ServerSocket { get; private set; }

        /// <summary>
        /// The listener thread for this client
        /// </summary>
        public Thread ListenThread { get; set; }

        /// <summary>
        /// The main application thread for this client
        /// </summary>
        public Thread MainThread { get; set; }
        /// <summary>
        /// Stores the process id for this client.
        /// </summary>
        public Process ReversiClientProcess = null;
        #endregion

        #region Constructor

        public ReversiClientModel()
        {
            // Store the client process
            ReversiClientProcess = Process.GetCurrentProcess();
            MainThread = Thread.CurrentThread;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Function to establish a connection between this client and the game server
        /// </summary>
        /// <param name="status">Status message of the connection attempt</param>
        /// <returns></returns>
        public bool MakeConnection(out string status)
        {
            NetworkStream serverStream;

            // Otherwise try to make the connection
            try
            {
                // save the socket once the connection is made
                ServerSocket = ClientModel.Connect(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
                serverStream = ServerSocket.GetStream();
            }
            catch (ArgumentNullException excep)
            {
                status = "ArgumentNullException: {0}";
                Console.WriteLine(status, excep);
                return false;
            }
            catch (SocketException excep)
            {
                status = "SocketException: {0}"; 
                Console.WriteLine(status, excep);
                return false;
            } catch
            {
                status = "Unable to connect to socket..";
                Console.WriteLine(status);
                return false;
            }

            status = "Connected to server...";
            return true;
        }
        #endregion

    }
}
