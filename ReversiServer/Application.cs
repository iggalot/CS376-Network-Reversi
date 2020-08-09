using ClientServerLibrary;
using Reversi.Models;
using System;
using System.Net.Sockets;

namespace ReversiServer
{
    /// <summary>
    /// The main entry point to the reversi server application
    /// </summary>
    public class Application
    {
        /// <summary>
        /// The primary server manager for this application
        /// </summary>
        public static ReversiServerManagerModel Manager { get; set; }
        static void Main()
        {
            Console.WriteLine("Launching Application");

            Manager = new ReversiServerManagerModel();
            Manager.ConfigureManager();
            Manager.ListenerSocket = Manager.StartManager();  // Returns the TcoListener for this server
            Manager.StartServer();

            // Now listen for incoming connections and add them to the connections list as they come in.
            // The server managers HandleServerThread will assign them to appropriate game and/or chat servers.
            while (!Manager.ShouldShutdown)
            {
                // Listen for connections
                TcpClient newTcpClientSocket = Manager.ListenForConnections();
                ReversiClientModel newTcpClientModel = new ReversiClientModel(newTcpClientSocket, null);

                // Determine what we should do with the connection and send the appropriate response
                newTcpClientModel = Manager.AcceptOrRefuseConnection(newTcpClientModel, out var status);

                // If the connection was accepted then add the model to the connected models list
                if (status == ConnectionStatusTypes.StatusConnectionAccepted)
                {
                    Manager.AddClientModelToConnectionList(newTcpClientModel);
                    Console.WriteLine("Server manager #" + Manager.Id + " has accepted client " + newTcpClientModel.Id + ".");
                }
                else if (status == ConnectionStatusTypes.StatusConnectionRefused)
                {
                    Console.WriteLine("Server manager #" + Manager.Id + " has refused client" + newTcpClientModel.Id + ".");
                }

                // List our connected clients for testing.
                Console.WriteLine(Manager.ListConnectedClients());

                // Dispose of the temporary client
                newTcpClientModel = null;
            }


            // Shutdown the server manager
            Manager.Shutdown();           
        }
    }
}
