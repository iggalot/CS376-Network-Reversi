using ClientServerLibrary;
using Reversi;
using Reversi.Models;
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
        /// This server
        /// </summary>
        public static Server GameServer { get; set; }

        /// <summary>
        /// A list of the connected clients
        /// </summary>
        public static List<TcpClient> ConnectedClients = new List<TcpClient>();
        /// <summary>
        /// The list of currently running games
        /// </summary>
        public static List<Game> RunningGames {get; set;} = new List<Game>();

        /// <summary>
        /// The staging area for all of our players
        /// </summary>
        public static List<Player> WaitingRoom { get; set; } = new List<Player>();

        /// <summary>
        /// A temporary staging area for when players are moved fro mthe waiting room to the game room
        /// </summary>
        public static List<Player> StagingArea { get; set; } = new List<Player>();

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
            // Setup our statemachine for this game
            ReversiStateMachine gs = new ReversiStateMachine();
            ReversiStateMachine.TestStateMachine();
            
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
            Console.WriteLine("... ChatServer: Chat server started");
            while(true)
            {
                Thread.Sleep(3000);
                Console.WriteLine("...ChatServer: chat server is idle");
            }
        }

        /// <summary>
        ///  The main thread for the game server
        /// </summary>
        private static void GameServerThread()
        {
            // Start our communication server listening for players
            GameServer = new Server(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);

            // Listen for a connection, and send acknowledgement when one is made
            Console.WriteLine("... GameServer: Game server started. Listening for connections...");

            TcpClient client;

            //TODO: Detect if there are any dead sockets.  If so, revise the count.
            // Now listen for connections
            // While there are less than total number of players...continue listening for new players
            while (true)
            {
                PacketInfo packet;
                client = GameServer.ListenForConnections(out packet);

                // If no socket, return to listening.
                if (client == null || packet == null)
                    continue;

                // Add to the connected sockets list
                ConnectedClients.Add(client);

                // Acknowledge the connection and determine if we should accept it or refuse it.
                AcknowledgeConnection(client, packet);

                // If we have two players currently waiting, create a game for them
                // Must make sure that count is greater than 0
                if ((WaitingRoom.Count % GlobalSettings.PlayersPerGame == 0) && (WaitingRoom.Count > GlobalSettings.PlayersPerGame-1))
                {
                    Console.WriteLine("... GameServer: " + ConnectedClients.Count.ToString() + " connections made ... Ready to begin game");

                    // Retrieve the placeholder players from the WaitingRoom and add them to the Staging Area
                    Player player1 = WaitingRoom[0];
                    StagingArea.Add(player1);

                    Player player2 = WaitingRoom[1];
                    StagingArea.Add(player2);

                    // Remove players from the Waiting room
                    WaitingRoom.Remove(player1);
                    WaitingRoom.Remove(player2);

                    // Create the game
                    // Populate the players
                    // Start the game
                    Thread gameThread = new Thread(InitializeMatchup);
                    gameThread.Start(StagingArea);
                } else
                {
                    Console.WriteLine("... GameServer: " + ConnectedClients.Count.ToString() + " connections currently. Waiting for additional connections.");

                }
            }

            // Once the game is over, shutdown the server
            GameServer.Shutdown();
        }

        /// <summary>
        /// Accepting a connection
        /// </summary>
        /// <param name="client">The client socket</param>
        /// <param name="packet">The packet of data created when the connection was first made in ListenForConnections</param>
        private static void AcceptConnection(TcpClient client, PacketInfo packet)
        {
            Console.WriteLine("... GameServer: Connection accepted for " + packet.Data);

            if (client != null)
            {
                // Prepare a connection acknowledgement from the server to the client
                int newID = NextId;
                NextId++; // increment the next ID counter

                // Create the packet for an accepted connection response, returning the player name and the new ID.
                packet.Id = newID;
                packet.Type = PacketType.PACKET_CONNECTION_ACCEPTED;

                // TODO: Handle any errors in transmission -- should this be in DataTransmission instead?
                if (!DataTransmission.SendData(client, packet))
                {
                    throw new SocketException();
                };


                // Add the user as a player to the waiting room.  Player number is undetermined at this point,
                // but will be set when the game actually begins.
                WaitingRoom.Add(new Player(Players.UNDEFINED, packet.Data, client));
            }
        }

        /// <summary>
        /// Refusing a connection
        /// </summary>
        /// <param name="client">The client socket</param>
        /// <param name="packet">The packet of data created when the connection was first made in ListenForConnections</param>
        private static void RefuseConnection(TcpClient client, PacketInfo packet)
        {
            Console.WriteLine("... GameServer: Maximum number of server connections exceeded. Refusing connection for " + packet.Data);

            if (client != null)
            {
                // Create the packet for an accepted connection response
                packet.Id = -1;
                packet.Data = RejectConnectionMessage;
                packet.Type = PacketType.PACKET_CONNECTION_REFUSED;

                // TODO: Handle any errors in transmission -- should this be in DataTransmission instead?
                if (!DataTransmission.SendData(client, packet))
                {
                    throw new SocketException();
                }
            }

            // Remove the player from the WaitingRoom
            foreach (Player p in WaitingRoom)
            {
                if (p.Socket == client)
                {
                    WaitingRoom.Remove(p);
                    break;
                }
            }
                    
            client.Close();                   // close the socket
            ConnectedClients.Remove(client);  // remove from the connected list
        }

        /// <summary>
        /// Function to determine whether a connection should be accepted or refused.
        /// </summary>
        /// <param name="client">The client socket</param>
        /// <param name="packet">The packet of data created when the connection was first made in ListenForConnections</param>
        private static void AcknowledgeConnection(TcpClient client, PacketInfo packet)
        {
            // Send a connection declined message for too many connections
            if (ConnectedClients.Count > GlobalSettings.MaxConnections)
            {
                RefuseConnection(client, packet);
            } else
            {
                AcceptConnection(client, packet);
            }
        }

        /// <summary>
        /// The main thread routine for each game
        /// </summary>
        /// <param name="data"></param>
        private static void InitializeMatchup(object data)
        {
            List<Player> player = (List<Player>)data;
            Player player1 = player[0];
            player1.ID = Players.PLAYER1;

            Player player2 = player[1];
            player2.ID = Players.PLAYER2;

            // Once two players have connected, start the game with the two players.
            foreach (Player item in player)
            {
                if (item.Socket.Connected)
                {
                    Console.WriteLine("... " + item.Name + " is " + item.ID);
                }
            }


            // Create the game instance and play the game between the two players...
            // TODO:  Add client information into the game...(socket references, data, etc?)
            ReversiGame game = new ReversiGame(player1, player2);

            game.CurrentPlayer = player1;
            game.CurrentOpponent = player2;
            Console.WriteLine("...... (GameThread (id: " + Game.GameID.ToString() + ") Matchup pairing complete. Beginning game...");

            // Signal the players that the game is starting...
            PacketInfo gameStartPacket = new PacketInfo(-1, "......(GameThread (id: " + Game.GameID.ToString() + ") Game is starting.", PacketType.PACKET_GAME_STARTING);

            


            //// Flush all sockets prior to starting the game.
            //NetworkStream clientStream1 = player1.Socket.GetStream();

            //clientStream1.Flush();
            //// TODO: Handle any errors in transmission -- should this be in DataTransmission instead?
            //if (!DataTransmission.SendData(player1.Socket, gameStartPacket))
            //{
            //    throw new SocketException();
            //};

            // Compile a list of the player sockets.
            List<TcpClient> playersSocketList = game.GetPlayersSockets();
            
            // Clear the socket streams for the game server since we are starting the game
            DataTransmission.FlushMultipleUsers(playersSocketList);

            // Broadcast the start of game message
            if(!DataTransmission.SendDataToMultipleUsers(playersSocketList, gameStartPacket))
            {
                throw new SocketException();
            }

            /////////////////////////////////////////
            // Broadcast the initial game board
            /////////////////////////////////////////
            // Signal the players that the game is starting...
            Console.WriteLine("...... (GameThread (id: " + Game.GameID.ToString() + ") Sending gameboard to clients...");

            PacketInfo gameboardPacket = new PacketInfo(-1, game.Gameboard.CreateGameboardPacketString(), PacketType.PACKET_GAME_STARTING);

            Thread.Sleep(1000);

            //// Clear the socket streams for the game server since we are broadcasting the gameboard
            DataTransmission.FlushMultipleUsers(playersSocketList);

            // Broadcast the first gameboard
            if(!DataTransmission.SendDataToMultipleUsers(playersSocketList, gameboardPacket))
            {
                throw new SocketException();
            }
            Console.WriteLine("Initial gameboard packet sent...");


            Console.WriteLine("......(GameThread(id: " + Game.GameID.ToString() + ") Game is Running");
            // The main game loop. Process individual moves here
            while (true)
            {
                // TODO: Check for dropped players, or dead sockets.
                Thread.Sleep(3000);

                // Listen for game moves
                Console.WriteLine("......(GameThread(id: " + Game.GameID.ToString() + ") Waiting to receive game move");
                PacketInfo gameMovePacket;

                // Ignore the opponent moves and send rejection message
                if(game.CurrentOpponent.Socket.GetStream().DataAvailable)
                {
                    Console.WriteLine("Checking for move from opponent...");
                    // If there was no data (or an UNDEFINED packet was returned), do nothing because the client isn't listening
                    // otherwise we send a MOVE_DENIED packet back to the opponent.
                    if (DataTransmission.ReceiveData(game.CurrentOpponent.Socket, out gameMovePacket))
                    {
                        if (gameMovePacket == null)
                        {
                            Console.WriteLine("Server: gameMovePacket was null for opponent");
                        }
                        else if (gameMovePacket.Type == PacketType.PACKET_UNDEFINED)
                        {
                            Console.WriteLine("Server: gameMovePacket type was UNDEFINED");
                        }
                        // send a denied response if a move was received
                        else if (gameMovePacket.Type == PacketType.PACKET_GAMEMOVE_REQUEST)
                        {
                            Console.WriteLine("Server: Move received");
                            DataTransmission.SendData(game.CurrentOpponent.Socket, new PacketInfo(-1, "It is not your move.", PacketType.PACKET_GAMEMOVE_DENIED));
                        } else
                        {
                            Console.WriteLine("Server: Invalid packet of type " + gameMovePacket.Type + " was received.");
                        }
                    }
                }

                // Now check the Current Player socket for a move
                if (game.CurrentPlayer.Socket.GetStream().DataAvailable)
                {
                    Console.WriteLine("Server: Checking for move from current player...");
                    DataTransmission.ReceiveData(game.CurrentPlayer.Socket, out gameMovePacket);

                    // If there was no data (or an UNDEFINED packet was returned, do nothing because the client isn't listening
                    // and cycle back to the beginning og the loop and continue listening
                    if ((gameMovePacket == null) || (gameMovePacket.Type == PacketType.PACKET_UNDEFINED))
                    {
                        // No data was received so we restart the listening cycle
                        continue;
                    }
                    else
                    {
                        // Determine if the move request was valid...
                        //TODO: Determine if move was valid...
                        DataTransmission.SendData(game.CurrentPlayer.Socket, new PacketInfo(-1, "Valid move detected.", 
                            PacketType.PACKET_GAMEMOVE_ACCEPTED));
                    }
                }
            }
        }
    }
}
