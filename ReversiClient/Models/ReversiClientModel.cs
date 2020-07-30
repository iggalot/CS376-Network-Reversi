using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
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
        public ReversiGame Game { get; set; }

        /// <summary>
        /// The player model associated with this client
        /// </summary>
        public Player ClientPlayer { get; set; }

        /// <summary>
        /// Flag to indicate if the client should shutdown
        /// </summary>
        public bool ClientShouldShutdown = false;

        /// <summary>
        /// Stores the object data for the last move sent to the server.
        /// </summary>
        public ReversiGameMove LastMove { get; set; }

        /// <summary>
        /// Returns the socket connection to the game server
        /// </summary>
        public TcpClient ServerSocket { get; private set; }

        /// <summary>
        /// Returns the socket connection to the game server
        /// </summary>
        public Thread ListenThread { get; set; }
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
                ServerSocket = Client.Connect(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
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
