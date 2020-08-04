using ClientServerLibrary;
using Settings;
using System;
using System.Net.Sockets;
using System.Threading;

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
        public static ServerManagerModel Manager { get; set; }
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
                TcpClient newTcpClientSocket = Manager.ListenForConnections();
                ClientModel newTcpClientModel = new ClientModel(newTcpClientSocket, null);

                // Determine what we should do with the connection and send the appropriate response
                ConnectionStatusTypes status;
                Manager.AcceptOrRefuseConnection(newTcpClientModel, out status);

                // If the connection was accepted then add the model to the connected models list
                if(status==ConnectionStatusTypes.STATUS_CONNECTION_ACCEPTED)
                {
                    Manager.AddClientModelToConnectionList(newTcpClientModel);
                    Console.WriteLine("Server manager #" + Manager.ID + " has accepted client " + newTcpClientModel.ID + ".");
                } else if (status==ConnectionStatusTypes.STATUS_CONNECTION_REFUSED)
                {
                    Console.WriteLine("Server manager #" + Manager.ID + " has refused client" + newTcpClientModel.ID + ".");
                }

                // TODO:  Why is client being closed at this point?

                Console.WriteLine ("Connections to application: " + Manager.ConnectedClientModelList.Count);
                Console.WriteLine(" -- New client model created with ID: " + newTcpClientModel.ID);
            }


            // Shutdown the server manager
            Manager.ServerShutdown();           
        }
    }
}
