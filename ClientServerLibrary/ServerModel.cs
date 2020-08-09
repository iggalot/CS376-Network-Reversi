using System;
using System.Collections.Generic;
using System.Threading;

namespace ClientServerLibrary
{
    /// <summary>
    /// Our primary server class that handles multiple client connections
    /// </summary>
    [Serializable]
    public class ServerModel : ClientServerInfoModel
    {
        #region Private Properties

        /// <summary>
        /// The dictionary of all known connections to this server / server manager
        /// </summary>
        public Dictionary<int, ClientModel> ConnectedClientModelList { get; } = new Dictionary<int, ClientModel>();
        /// <summary>
        /// The list of threads associated with this server
        /// </summary>
        private Dictionary<int, Thread> _threadsList = new Dictionary<int, Thread>();


        /// <summary>
        /// The type of this server <see cref="ServerTypes"/>
        /// </summary>
        private ServerTypes ServerType { get; set; } = ServerTypes.ServerUnknown;



        #endregion

        #region Public Properties


        /// <summary>
        /// The thread that controls the continuous update thread for the server instance
        /// </summary>
        [NonSerialized()] public Thread UpdateThread;

        /// <summary>
        /// The type of this server
        /// </summary>
        public ServerTypes Type { get; private set; }

        /// <summary>
        /// The IP address for the server
        /// </summary>
        public static string Address { get; private set; }

        /// <summary>
        /// The port that this server is running on
        /// </summary>
        public static Int32 Port { get; private set; }

        // Tells our server that it should shutdown
        public const string RejectConnectionMessage = "The server has rejected your connection. Please try again later.";
        public const string AcceptConnectionMessage = "Welcome to the Reversi / Othello server!";

        public bool IsRunning { get; set; } = true;

        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for this server
        /// </summary>
        /// <param name="serverType">The type of server</param>
        /// <param name="address">The address for this server</param>
        /// <param name="port">The port for this server</param>
        public ServerModel(ServerTypes serverType, string address, Int32 port) : base(null, null)
        {
            Type = serverType;
            Address = address;
            Port = port;

            Console.WriteLine("-- Creating server of type " + serverType + " on " + address + ": " + port);

            LaunchUpdateServerThread();
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// The thread callback routine for this server
        /// </summary>
        public virtual void HandleServerThread() 
        {
            Console.WriteLine("(ServerModel:) HandleServerThread");
        }

        /// <summary>
        /// Start the server and start listening for connections
        /// </summary>
        public virtual void StartServer() 
        {
            IsRunning = true;
            Console.WriteLine("------ (ServerModel:) Starting server #" + Id.ToString());

            Console.WriteLine("ServerModel: Starting the main HandleServerThread()");

            Thread handleThread = new Thread(HandleServerThread);
            handleThread.Start();


        }
        public virtual void Update()
        {
//            Console.WriteLine("------- Checking for server updates on server " + ID.ToString());
        }

        #endregion

        #region Public Methods
        public void LaunchUpdateServerThread()
        {
            Console.WriteLine("------- Launching server " + Id + " update thread");
            UpdateThread = new Thread(RunUpdateThread);
            UpdateThread.Start();
        }


        /// <summary>
        /// Adds a client model to this servers participant list
        /// </summary>
        /// <param name="clientModel"></param>
        public void AddClientModelToServer(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            ConnectedClientModelList.Add(clientModel.Id, clientModel);
        }

        /// <summary>
        /// Remove the client from the server
        /// </summary>
        /// <param name="clientModel"></param>
        public void RemoveClientModelFromServer(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            if(ConnectedClientModelList.ContainsKey(clientModel.Id))
                ConnectedClientModelList.Remove(clientModel.Id);
        }

        /// <summary>
        /// Gets the oldest client model (lowest ID) from the list.
        /// </summary>
        /// <returns></returns>
        public ClientModel GetOldestClientModelFromConnectedList()
        {
            // Create a list of the keys
            List<int> list = new List<int>(ConnectedClientModelList.Keys);

            // Check that there is at least on item in the list
            if (list.Count == 0)
                return null;

            // Sort the list in ascending order
            list.Sort();

            // Retrieve the first (oldest) item in the list by smallest key value
            int oldestIndex = list[0];

            // Now retrieve the model from the dictionary
            ConnectedClientModelList.TryGetValue(oldestIndex, out var result);

            return result;
        }




        #endregion




        /// <summary>
        /// A function for shutting down the server.
        /// </summary>
        public virtual void Shutdown()
        {
            //// Shutdown and end connection         
            ListenerSocket.Stop();
            Console.WriteLine(" >> Server " + Id + " is shutting down...");
            Console.ReadLine();
        }

        ///// <summary>
        ///// A server routine to broadcast a string message to all clients currently connected to
        ///// the server.
        ///// </summary>
        ///// <param name="message"></param>
        //public static void BroadcastToAll(string message, string uName, bool flag)
        //{
        //    foreach(DictionaryEntry client in ClientSocketList)
        //    {
        //        TcpClient broadcastSocket;
        //        broadcastSocket = (TcpClient)client.Value;

        //        if (broadcastSocket == null || !broadcastSocket.Connected)
        //        {
        //            ClientSocketList.Remove(client);
        //            continue;
        //        }

        //        try
        //        {
        //            NetworkStream broadcastStream = broadcastSocket.GetStream();
        //            Byte[] broadcastBytes = null;

        //            if (flag == true)
        //            {
        //                broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + message);
        //            }
        //            else
        //            {
        //                broadcastBytes = Encoding.ASCII.GetBytes(message);
        //            }

        //            broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
        //            broadcastStream.Flush();
        //        } 
        //        catch (System.IO.IOException e)
        //        {
        //            // If our socket has disconnected, remove the client
        //            continue;
        //        }
        //    }
        //}

        public virtual void RunUpdateThread()
        {
            Console.WriteLine("- Update thread for server " + Id + " created");
            while (!ShouldShutdown)
            {
                // If we have clients currently connected, check for updates
                if(ConnectedClientModelList.Count > 0)
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
    }

    ///// <summary>
    ///// Class to handle each client request separately.
    ///// </summary>
    //public class handleClient
    //{
    //    TcpClient clientSocket;
    //    // The server that created this client thread
    //    Hashtable clientsList;
    //    string clientNum;

    //    /// <summary>
    //    /// The function to start the client thread.
    //    /// </summary>
    //    /// <param name="inClientSocket">The associated client socket</param>
    //    /// <param name="strClientNum">String containing the client's number</param>
    //    public void startClient(TcpClient inClientSocket, string strClientNum, Hashtable clist)
    //    {
    //        this.clientSocket = inClientSocket;
    //        this.clientNum = strClientNum;
    //        this.clientsList = clist;
    //        Thread ctThread = new Thread(()=>doChat());
    //        ctThread.Start();
    //    }

    //    ///// <summary>
    //    ///// The main thread function for each connected client.
    //    ///// </summary>
    //    //private void doChat()
    //    //{
    //    //    int requestCount = 0;
    //    //    byte[] bytesFrom = new byte[65536];
    //    //    string dataFromClient = null;
    //    //    Byte[] sendBytes = null;
    //    //    string serverResponse = null;
    //    //    string rCount = null;
    //    //    requestCount = 0;

    //    //    while ((true))
    //    //    {
    //    //        if (!clientSocket.Connected)
    //    //            break;

    //    //        try
    //    //        {
    //    //            // increment our response counter
    //    //            requestCount = requestCount + 1;
    //    //            NetworkStream networkStream = clientSocket.GetStream();
    //    //            networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
    //    //            dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
    //    //            dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
    //    //            Console.WriteLine(" >> " + "From client - " + clientNum + ": " + dataFromClient);
    //    //            rCount = Convert.ToString(requestCount);

    //    //            ServerModel.BroadcastToAll(dataFromClient, clientNum, true);
    //    //        }

    //    //        catch (System.IO.IOException ex)
    //    //        {
    //    //            Console.WriteLine(" >> Client #" + clientNum + " has disconnected.");
    //    //            // Close the socket
    //    //            clientSocket.Close();
    //    //            break;
    //    //        }
    //    //        catch (ArgumentOutOfRangeException ex)
    //    //        {
    //    //            break;
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            Console.WriteLine(" >> " + ex.ToString());
    //    //            break;
    //    //        }
    //    //    }
    //    //}
    //}


}

