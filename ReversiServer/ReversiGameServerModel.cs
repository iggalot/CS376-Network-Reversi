using ClientServerLibrary;
using GameObjects.Models;
using Reversi;
using Reversi.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Threading;

namespace ReversiServer
{
    public class ReversiGameServerModel : ServerModel
    {
        #region Private Properties

        private Dictionary<int, ReversiGameModel> _runningGameModelsList = new Dictionary<int, ReversiGameModel>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of currently running games, using the gameID as an entry and the ReversiGame as the value
        /// </summary>
        public static Dictionary<int, ReversiGameModel> RunningGames = new Dictionary<int, ReversiGameModel>();

        /// <summary>
        /// The staging area for all of our players
        /// </summary>
        public static List<ReversiClientModel> WaitingList { get; set; } = new List<ReversiClientModel>();

        /// <summary>
        /// A temporary staging area for when players are moved from the waiting room to the game room
        /// </summary>
        public static List<ReversiClientModel> StagingArea { get; set; } = new List<ReversiClientModel>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="address">The address to the server</param>
        /// <param name="port">The port for the server</param>
        public ReversiGameServerModel(string address, Int32 port ) : base(ServerTypes.ServerGameserver, address, port)
        {
            // TODO: Special ReversiGameServer setup here.
            Console.WriteLine("---- Creating a new Reversi Game Server with ID:" + Id.ToString());

            this.StartServer();
        }

        #endregion

        #region Public Methods

        public override void StartServer()
        {
            base.StartServer();

            // TODO:  Start threads
            // TODO:  Create listener
            // TODO:  Create game model
        }



        /// <summary>
        /// Refusing a connection.
        /// Returns an UNDEFINED player packet
        /// </summary>
        /// <param name="model">The client model</param>
        public ReversiClientModel RefuseConnection(ReversiClientModel model)
        {
            //PlayerModel newplayer = DataTransmission.DeserializeData<PlayerModel>(client);

            //Console.WriteLine("... GameServer: Refusing connection for " + newplayer.Name + ". Maximum number of server connections exceeded.");

            //if (client != null)
            //{
            //    newplayer.IDType = Players.UNDEFINED;
            //    try
            //    {
            //        DataTransmission.SerializeData<PlayerModel>(newplayer, client);
            //    }
            //    catch
            //    {
            //        throw new SocketException();
            //    }
            //}

            //Remove the player from the WaitingList
            //foreach (PlayerModel p in WaitingList)
            //{
            //    if (p.Socket == client)
            //    {
            //        WaitingList.Remove(p);
            //        break;
            //    }
            //}

            //client.Close();                   // close the socket
            return model;
        }

        #endregion

        #region  Private Methods
        ///// <summary>
        ///// Returns the socket associated with a playerID.
        ///// </summary>
        ///// <param name="playerId"></param>
        ///// <returns></returns>
        //private static TcpClient GetSocketByPlayerId(int playerId)
        //{
        //    ConnectedClients.TryGetValue(playerId, out var clientSocket);

        //    return clientSocket;
        //}

        /// <summary>
        /// Returns the socket associated with a playerID.
        /// </summary>
        /// <param name="gameId">The game id number</param>
        /// <returns></returns>
        private static GameModel GetGameByGameId(int gameId)
        {
            RunningGames.TryGetValue(gameId, out var game);

            return game;
        }
        #endregion

        #region Thread Callbacks

        /// <summary>
        /// The handler server thread
        /// </summary>
        public override void HandleServerThread()
        {

            //TODO: Detect if there are any dead sockets.  If so, revise the count.
            // Now listen for connections
            // While there are less than total number of players...continue listening for new players
            while (ShouldShutdown == false)
            {
                //// Check if the Socket is still connected.  If not, exit and gracefully shutdown the thread.
                //foreach (KeyValuePair<int, TcpClient> connection in ConnectedClients)
                //{
                //    if (!DataTransmission.SocketConnected((connection.Value).Client))
                //    {
                //        // TODO:  What to do if a socket has closed
                //        // 1. Notify other members of the game that a player has left
                //        // 2. Pause the game...
                //        // 3. Remove the player from the game and chat room
                //        //      a.  Are there any players remaining in the game?
                //        // 4. Resend the updated board to remaining players...
                //        // 5. Fill empty game slots with new connections...
                //        //      a.  If user name matches, allow them to reconnect?
                //    }
                //}
            }

            // Once the game is over, shutdown the server
            Shutdown();
        }

        /// <summary>
        /// Moves a player from the waiting room to the staging area as a game begins.
        /// </summary>
        private static void MovePlayersFromWaitingToStaging()
        {
            for(int i = ReversiSettings.ReversiPlayersPerGame - 1; i>=0; i--)
            {
                StagingArea.Add(WaitingList[i]);
                WaitingList.RemoveAt(i);
            }
        }

        ///// <summary>
        ///// Removes a player from the staging area to the gameroom.
        ///// </summary>
        //private static void RemovePlayersFromStaging()
        //{
        //    for (int i = ReversiSettings.ReversiPlayersPerGame - 1; i >= 0; i--)
        //    {
        //        StagingArea.RemoveAt(i);
        //    }
        //}



        /// <summary>
        /// The main thread routine for each game
        /// </summary>
        /// <param name="data">The game staging area list of client models</param>
        private void InitializeMatchup(object data)
        {
            List<ReversiClientModel> clientModels = (List<ReversiClientModel>)data;

            // Move players to the gameroom
            Console.WriteLine("... GameServer: Matchup pairing complete. Game is ready to start.");


            //// Create the game instance and play the game between the players...
            ReversiGameModel game = new ReversiGameModel(clientModels);
            RunningGames.Add(game.GameId, game);  // add the game to the dictionary of running games

            Console.WriteLine("... GameServer: Creating game thread (id: " + game.GameId.ToString() + ") Beginning game...");
            //            Console.WriteLine("..... Game #" + game.GameId + " Participants\n" + game.DisplayGamePlayers());


            // Send the game object to each of the clients.
            foreach (KeyValuePair<int,ClientModel> item in game.CurrentPlayersList)
            {
                ReversiClientModel client = (ReversiClientModel) item.Value;

                // Send the game data to each of the players
                Console.WriteLine("Sending initial game matchup to players");
                DataTransmission.SerializeData<ReversiGameModel>(game, client.ConnectionSocket);

                StagingArea.Remove(client);
            }

            int temp = game.CurrentPlayersList.Count;

            //// The main game loop. Process individual moves here
            List<TcpClient> sockets = game.GetPlayersSocketList();
            while (!game.GameIsOver)
            {
                // If the game is paused ignore the user input
                if (game.GameIsPaused == true)
                    continue;

                // If the current turn is valid and complete, switch to the next player
                if (game.TurnComplete)
                {
                    game.NextPlayer();
                    game.TurnComplete = false;

                    SendGameToAll(game);
                }

                List<ClientModel> disconnectList = new List<ClientModel>();
                foreach (KeyValuePair<int,ClientModel> item in game.CurrentPlayersList)
                {
                    ClientModel client = item.Value;
                    Socket s = client.ConnectionSocket.Client;

                    // Check for disconnected sockets
                    if (!SocketConnected(s))
                    {
                        Console.WriteLine("GameServer: (GameID #" + game.GameId + ")");

                        disconnectList.Add(client);
                    }
                }

                // Remove any disconnected players from the game...
                foreach (ClientModel disconnectClient in disconnectList)
                {
                    ClientDisconnectedEventArgs args = new ClientDisconnectedEventArgs
                    {
                        client = disconnectClient,
                        TimeOfDisconnect = DateTime.Now
                    };
                    game.GameIsPaused = true;
                    OnClientDisconnected(args);

                    game.RemovePlayerFromGame(((ClientModel) disconnectClient));
                    try
                    {
                        disconnectClient.ConnectionSocket.Close();
                    }
                    catch
                    {
                        // Do nothing
                    }
                }

                // Now proceed through the current player list
                foreach (KeyValuePair<int,ClientModel> item in game.CurrentPlayersList)
                {
                    NetworkStream stream;
                    try
                    {
                        stream = item.Value.ConnectionSocket.GetStream();
                    }
                    catch (ObjectDisposedException e)
                    {
                        // Catches a disposed socket possibility here in case it hasn't been fully disposed yet.
                        continue;
                    }

                    if (stream.DataAvailable)
                    {
                        GameMoveModel move = DataTransmission.DeserializeData<GameMoveModel>(item.Value.ConnectionSocket);
                        Console.WriteLine("GameServer: (GameID #" + game.GameId + ") Player ID#" + move.ByPlayer + " move request received");

                        if (move.ByPlayer == game.CurrentPlayer)
                        {
                            game.CurrentMoveIndex = move.MoveIndex;

                            // Check that the move was valid.
                            if (game.PlayTurn())
                            {
                                Console.WriteLine("GameServer: (GameID #" + game.GameId + ") Player ID#" + move.ByPlayer + " submitted a valid move");
                                game.TurnComplete = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("GameServer: (GameID #" + game.GameId + ") Move received by opponent.  Ignoring...");
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Sends the current game state to all current players...
        /// </summary>
        /// <param name="game"></param>
        private static void SendGameToAll(ReversiGameModel game)
        {
            // Send the game object to each of the clients.
            foreach (KeyValuePair<int,ClientModel> item in game.CurrentPlayersList)
            {
                // Send the game data to each of the players
                Console.WriteLine("Sending initial game matchup to players");
                DataTransmission.SerializeData<ReversiGameModel>(game, item.Value.ConnectionSocket);
            }
        }

        //private static List<TcpClient> GatherPlayerSocketList(List<PlayerModel> p)
        //{
        //    List<TcpClient> temp = new List<TcpClient>();

        //    // Once two players have connected, start the game with the two players.
        //    foreach (PlayerModel item in p)
        //    {
        //        TcpClient socket = GetSocketByPlayerID(item.PlayerID);
        //        Console.WriteLine("...... " + item.Name + " is " + item.IDType);
        //        item.Socket = socket;  // save the socket reference for the game
        //        temp.Add(socket);
        //    }

        //    return temp;
        //}

        public override void Update()
        {
            var temp = ConnectedClientModelList;

            ReversiClientModel client = (ReversiClientModel)GetOldestClientModelFromConnectedList();
            Console.WriteLine(client.ClientPlayer.DisplayPlayerInfo());


            if (!WaitingList.Contains(client))
            {
                WaitingList.Add(client);
                RemoveClientModelFromServer(client);
            }

            // If insufficient players have joined, add to wait list...
            if (WaitingList.Count < ReversiSettings.ReversiPlayersPerGame)
            {
                // Do nothing here
            }


            // If two players and no running game, start the game
            if ((WaitingList.Count == ReversiSettings.ReversiPlayersPerGame) && RunningGames.Count == 0)
            {
                Console.WriteLine("-- GameServer: Creating 1st game");

                MovePlayersFromWaitingToStaging();

                // Create the game thread to handle this matchup and
                // Populate the players                    
                Thread gameThread = new Thread(InitializeMatchup);

                // Start the game
                gameThread.Start(StagingArea);
            }

            // If a game is already running, add the player to the waiting list...
            if (RunningGames.Count > 0)
            {
                Console.WriteLine("-- GameServer:  A game is already running so add client to waiting list");
            }

            // If someone already in the waiting list, and another player joins, allow option for players to start their own game...
            if (WaitingList.Count == ReversiSettings.ReversiPlayersPerGame)
            {
                Console.WriteLine("-- Sending option to Wait List for additional games to be created...");
                // TODO:  Send choice to waiting list about whether to keep waiting or to start a new game...
                // Continue waiting...
                // Or start a game with waiting list 
            }

            string str = string.Empty;
            str += "-------------------------------\n";
            foreach (ReversiClientModel item in WaitingList)
            {
                str += item.ClientPlayer.PlayerId + " -- " + item.ClientPlayer.Name + "\n";
            }
            str += "-------------------------------\n";
            Console.WriteLine(str);
        }

        public override void RunUpdateThread()
        {
            // TODO:  Establish the updating that is required by a reversi game server...
            // 1. Check for empty (but started) games
            // 2. If one player missing...pause game and notify other user
            //      a.  Allow a new player to take over
            // 3. If both players missing... close the gamee?







            Console.WriteLine("- Update thread for server " + Id + " created");
            while (!ShouldShutdown)
            {
                // If we have clients currently connected, check for updates
                if (ConnectedClientModelList.Count > 0)
                {
                    Console.WriteLine("Updating server " + Id);
                    this.Update();
                }

                // Cause the the update thread to sleep for a specified duration
                Thread.Sleep(ServerSettings.ServerUpdatePulseDelay);

            }

            // End the running thread

            UpdateThread.Join();
        }

        #endregion


    }
}
