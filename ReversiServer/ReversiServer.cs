using ClientServerLibrary;
using Reversi;
using Settings;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ReversiServer
{
    public class ReversiServer : Server
    {
        /// <summary>
        /// The list of threads running a game server
        /// </summary>
        public static List<Thread> GameThreads { get; set; } = new List<Thread>();

        /// <summary>
        /// The list of threads runnign a chat server
        /// </summary>
        public static List<Thread> ChatThreads { get; set; } = new List<Thread>();

        /// <summary>
        /// The next id for the server to use
        /// </summary>
        public static int NextId { get; set; } = 0;

        static void Main()
        {
            // Start the game server thread
            ThreadStart gameStart = new ThreadStart(GameServerThread);
            Thread gameThread = new Thread(gameStart);
            Console.WriteLine("Starting game server...");
            gameThread.Start();
            GameThreads.Add(gameThread);

            // Uncomment these lines to create the chat server thread
            //// Start the chat server thread
            //ThreadStart chatStart = new ThreadStart(ChatServerThread);
            //Thread chatThread = new Thread(chatStart);
            //Console.WriteLine("Starting chat server...");
            //chatThread.Start();
            //ChatThreads.Add(chatThread);

            gameThread.Join();

            // Uncomment this line to end the game when the chat thread completes.            
            //chatThread.Join();
        }

        /// <summary>
        /// The main thread for the chat server
        /// </summary>
        private static void ChatServerThread()
        {
            Console.WriteLine("... chat server started");
            while(true)
            {
                Thread.Sleep(3000);
                Console.WriteLine("...... chat server is idle");
            }
        }

        /// <summary>
        ///  The main thread for the game server
        /// </summary>
        private static void GameServerThread()
        {
            List<TcpClient> connectedClients = new List<TcpClient>();
            // Start our communication server listening for players
            Server server = new Server(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);

            // Listen for a connection
            Console.WriteLine("... Game server started. Listening for connections...");
            TcpClient client = server.ListenForConnections();
            if(client != null)
                connectedClients.Add(client);

            NetworkStream serverStream = client.GetStream();
            // Prepare a connection acknowledgement from the server to the client
            int newID = NextId;
            NextId++;
            PacketInfo packet = new PacketInfo(newID, "Test Response from Server", PacketType.PACKET_CONNECTION_ACCEPTED);
            string packetString = packet.FormPacket();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(packetString);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            //TODO: Detect if there are any dead sockets.  If so, revise the count.
            // Now listen for connections
            // While there are less than total number of players...continue listening for new players
            //while(connectedClients.Count < GlobalSettings.PlayersPerGame)
            //{
            //    Console.WriteLine("... " + connectedClients.Count.ToString() + " connections currently. Waiting for additional connections.");
            //    client = server.ListenForConnections();
            //    if(client != null)
            //        connectedClients.Add(client);
            //}

            Console.WriteLine("... " + connectedClients.Count.ToString() + " connections made ... Ready to begin game");
            // Once two players have connected, start the game with the two players.
            int count = 1;
            foreach(TcpClient item in connectedClients)
            {
                if(item.Connected)
                {
                    Console.WriteLine("... Connection " + item + " is Player " + count);
                    count++;
                }                    
            }

            Console.WriteLine("... Beginning game");

            // Create the game instance and play the game between the two players...
            // TODO:  Add client information into the game...(socket references, data, etc?)
            ReversiGame game = new ReversiGame(2);

            // The main game loop
            while(true)
            {
                // Check for dropped players, or dead sockets.
                Thread.Sleep(3000);
                Console.WriteLine("...... Game is Running");
            }

         
            // Once the game is over, shutdown the server
            server.Shutdown();
        }
    }
}
