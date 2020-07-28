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
        #region Public Properties
        /// <summary>
        /// This server
        /// </summary>
        public static Server GameServer { get; set; }

        /// <summary>
        /// A list of the connected clients using the playerID as an entry and the corresponding socket as the value
        /// </summary>
        public static Dictionary<int, TcpClient> ConnectedClients = new Dictionary<int, TcpClient>();

        /// <summary>
        /// The list of currently running games, using the gameID as an entry and the ReversiGame as the value
        /// </summary>
        public static Dictionary<int, ReversiGame> RunningGames = new Dictionary<int, ReversiGame>();

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

        /// <summary>
        /// Flag to indicate that the server should shutdown
        /// </summary>
        public static bool ServerShouldShutDown = false;

        #endregion

        #region Constructor

        static void Main()
        {
            // Setup our statemachine for this game
 //           ReversiStateMachine gs = new ReversiStateMachine();
 //           ReversiStateMachine.TestStateMachine();
            
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

        #endregion

        #region Public Methods


        private static void AcceptConnection(TcpClient client)
        {
            Player newplayer = DataTransmission.DeserializeData<Player>(client);

            Console.WriteLine("... GameServer: Connection accepted for " + newplayer.Name + " as " + newplayer.IDType);

            if (client != null)
            {
                // Prepare a connection acknowledgement from the server to the client
                int newID = NextId;
                NextId++; // increment the next IDType counter
                newplayer.PlayerID = newID;

                // Create the packet for an accepted connection response, returning the player name and the new IDType.
                if (newID % 2 == 0)
                    newplayer.IDType = Players.PLAYER1;
                else
                    newplayer.IDType = Players.PLAYER2;

                try
                {
                    DataTransmission.SerializeData<Player>(newplayer, client);
                }
                catch
                {
                    throw new SocketException();
                }

                // Add the user as a player to the waiting room.  Player number is undetermined at this point,
                // but will be set when the game actually begins.
                // Add to the connected sockets list
                ConnectedClients.Add(newID, client);

                // Add the player to the waiting room
                WaitingRoom.Add(newplayer);
            }
        }

        /// <summary>
        /// Refusing a connection.
        /// Returns an UNDEFINED player packet
        /// </summary>
        /// <param name="client">The client socket</param>
        private static void RefuseConnection(TcpClient client)
        {
            Player newplayer = DataTransmission.DeserializeData<Player>(client);

            Console.WriteLine("... GameServer: Refusing connection for " + newplayer.Name + ". Maximum number of server connections exceeded.");

            if (client != null)
            {
                newplayer.IDType = Players.UNDEFINED;
                try
                {
                    DataTransmission.SerializeData<Player>(newplayer, client);
                }
                catch
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
        }

        /// <summary>
        /// Function to determine whether a connection should be accepted or refused.
        /// </summary>
        /// <param name="client">The client socket</param>
        /// <param name="packet">The packet of data created when the connection was first made in ListenForConnections</param>
        private static void AcknowledgeConnection(TcpClient client)
        {
            int timeoutCount = 0;
            while ((timeoutCount > 150) || (!client.GetStream().DataAvailable))
            {
                Thread.Sleep(200);
                timeoutCount++;
            }

            if (timeoutCount > 150)
            {
                Console.WriteLine("Connection has timedout");
                return;
            }

            // Send a connection declined message for too many connections
            if (ConnectedClients.Count > GlobalSettings.MaxConnections)
            {
                RefuseConnection(client);
            }
            // Otherwise accept the connection
            else
            {
                AcceptConnection(client);
            }
        }


        #endregion

        #region  Private Methods
        /// <summary>
        /// Returns the socket associated with a playerID.
        /// </summary>
        /// <param name="player_id"></param>
        /// <returns></returns>
        private static TcpClient GetSocketByPlayerID(int player_id)
        {
            TcpClient client_socket;
            ConnectedClients.TryGetValue(player_id, out client_socket);

            return client_socket;
        }

        /// <summary>
        /// Returns the socket associated with a playerID.
        /// </summary>
        /// <param name="game_id">The game id number</param>
        /// <returns></returns>
        private static ReversiGame GetGameByGameID(int game_id)
        {
            ReversiGame game;
            RunningGames.TryGetValue(game_id, out game);

            return game;
        }
        #endregion

        #region Thread Callbacks

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
            while (!ServerShouldShutDown)
            {
                //PacketInfo packet;
                client = GameServer.ListenForConnections();

                // If no socket, return to listening.
                if (client == null)
                    continue;

                // Acknowledge the connection and determine if we should accept it or refuse it.
                AcknowledgeConnection(client);

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

                    // Create the game thread to handle this matchup and
                    // Populate the players                    
                    Thread gameThread = new Thread(InitializeMatchup);

                    // Start the game
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
        /// The main thread routine for each game
        /// </summary>
        /// <param name="data"></param>
        private static void InitializeMatchup(object data)
        {
            List<Player> players = new List<Player>();
            List<Player> temp = (List<Player>)data;

            foreach (Player p in temp)
                players.Add(p);

            List<TcpClient> sockets = new List<TcpClient>();

            Player player1 = players[0];
            Player player2 = players[1];

            StagingArea.Remove(player1);
            StagingArea.Remove(player2);

            // Once two players have connected, start the game with the two players.
            foreach (Player item in players)
            {
                TcpClient socket = GetSocketByPlayerID(item.PlayerID);
                Console.WriteLine("...... " + item.Name + " is " + item.IDType);
                item.Socket = socket;  // save the socket reference for the game
                sockets.Add(socket);
            }

            if(sockets.Count != GlobalSettings.PlayersPerGame)
            {
                Console.WriteLine("Error retrieving sockets for all players of this game.");
                return;
            } else
            {
                Console.WriteLine("... GameServer: Matchup pairing complete.");
            }

            // Create the game instance and play the game between the two players...
            ReversiGame game = new ReversiGame(player1, player2);
            RunningGames.Add(game.GameID, game);  // add the game to the dictionary of running games

            Console.WriteLine("... GameServer: Creating game thread (id: " + game.GameID.ToString() + ") Beginning game...");

            // Send the game object to each of the players.
            foreach (TcpClient client in sockets)
            {
                // Send the game data to each of the players
                DataTransmission.SerializeData<ReversiGame>(game, client);
            }

            //// The main game loop. Process individual moves here
            while(!game.GameIsOver)
            {
                // If the current turn is valid and complete, switch to the next player
                if (game.TurnComplete)
                {
                    game.NextPlayer();
                    game.TurnComplete = false;

                    // Send the update gameboard to each player
                    foreach(TcpClient client in sockets)
                    {
                        DataTransmission.SerializeData<ReversiGame>(game, client);
                    }
                }
                
                // Listen for moves from each player
                foreach (TcpClient client in sockets)
                {
                    if(client == null)
                    {
                        // TODO: Determine which client has disconnected
                        Console.WriteLine("GameServer: (GameID #" + game.GameID + ") A client has disconnected");
                    }
                    NetworkStream stream = client.GetStream();

                    if (stream.DataAvailable)
                    {
                        ReversiGameMove move = DataTransmission.DeserializeData<ReversiGameMove>(client);
                        Console.WriteLine("GameServer: (GameID #" + game.GameID + ") Player ID# move request" + move.ByPlayer + " received");

                        if(move.ByPlayer == game.CurrentPlayer)
                        { 
                            game.CurrentMoveIndex = move.MoveIndex;

                            // Check that the move was valid.
                            if (game.PlayTurn())
                            {
                                Console.WriteLine("GameServer: (GameID #" + game.GameID + ") Player ID#" + move.ByPlayer + " submitted a valid move");
                                game.TurnComplete = true;
                            }
                        } else
                        {
                            Console.WriteLine("GameServer: (GameID #" + game.GameID + ") Move received by opponent.  Ignoring...");
                        }
                    }
                }


            }
            //while (true)
            //{
            //    // Test receiving the serialized data....
            //    ReversiGame newgame = DataTransmission.DeserializeData<ReversiGame>(playersSocketList[0]);

            //    // TODO: Check for dropped players, or dead sockets.
            //    Thread.Sleep(3000);

            //    // Listen for game moves
            //    Console.WriteLine("......(GameThread(id: " + Game.GameID.ToString() + ") Waiting to receive game move");
            //    PacketInfo gameMovePacket;

            //    // Ignore the opponent moves and send rejection message
            //    if(game.CurrentOpponent.Socket.GetStream().DataAvailable)
            //    {
            //        Console.WriteLine("Checking for move from opponent...");
            //        // If there was no data (or an UNDEFINED packet was returned), do nothing because the client isn't listening
            //        // otherwise we send a MOVE_DENIED packet back to the opponent.
            //        if (DataTransmission.ReceiveData(game.CurrentOpponent.Socket, out gameMovePacket))
            //        {
            //            if (gameMovePacket == null)
            //            {
            //                Console.WriteLine("Server: gameMovePacket was null for opponent");
            //            }
            //            else if (gameMovePacket.Type == PacketType.PACKET_UNDEFINED)
            //            {
            //                Console.WriteLine("Server: gameMovePacket type was UNDEFINED");
            //            }
            //            // send a denied response if a move was received
            //            else if (gameMovePacket.Type == PacketType.PACKET_GAMEMOVE_REQUEST)
            //            {
            //                Console.WriteLine("Server: Move received");
            //                DataTransmission.SendData(game.CurrentOpponent.Socket, new PacketInfo(-1, "It is not your move.", PacketType.PACKET_GAMEMOVE_DENIED));
            //            } else
            //            {
            //                Console.WriteLine("Server: Invalid packet of type " + gameMovePacket.Type + " was received.");
            //            }
            //        }
            //    }

            //    // Now check the Current Player socket for a move
            //    if (game.CurrentPlayer.Socket.GetStream().DataAvailable)
            //    {
            //        Console.WriteLine("Server: Checking for move from current player...");
            //        DataTransmission.ReceiveData(game.CurrentPlayer.Socket, out gameMovePacket);

            //        // If there was no data (or an UNDEFINED packet was returned, do nothing because the client isn't listening
            //        // and cycle back to the beginning og the loop and continue listening
            //        if ((gameMovePacket == null) || (gameMovePacket.Type == PacketType.PACKET_UNDEFINED))
            //        {
            //            // No data was received so we restart the listening cycle
            //            continue;
            //        }
            //        else
            //        {
            //            // Determine if the move request was valid...
            //            //TODO: Determine if move was valid...
            //            DataTransmission.SendData(game.CurrentPlayer.Socket, new PacketInfo(-1, "Valid move detected.", 
            //                PacketType.PACKET_GAMEMOVE_ACCEPTED));
            //        }
            //    }
            //}
        }

        #endregion



    }
}
