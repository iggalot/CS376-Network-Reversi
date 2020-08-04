using ClientServerLibrary;
using Settings;
using System;
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
            Manager.StartManager();
            Manager.StartServer();

            // Create a GameServer and ChatServer for this application
            Manager.AddServer(new ReversiGameServerModel(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer));
            Manager.AddServer(new ReversiChatServerModel(GlobalSettings.ServerAddress, GlobalSettings.Port_ChatServer));
            Manager.AddServer(new ReversiGameServerModel(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer));

            while (!Manager.ShouldShutdown)
            {
                Manager.AddClientSocketToConnectionList(Manager.ListenForConnections());
///                Manager.Update();

               // Manager.ListenForConnections();
               // Manager.MakeConnection();
            }


            // Shutdown the server manager
            Manager.ServerShutdown();           
        }
    }
}
