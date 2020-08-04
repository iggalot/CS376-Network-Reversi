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
        /// The dictionary of client sockets that are connected to this server
        /// </summary>
        private Dictionary<int, ClientModel> participantClientList = new Dictionary<int, ClientModel>();

        /// <summary>
        /// The list of threads associated with this server
        /// </summary>
        private Dictionary<int, Thread> threadsList = new Dictionary<int, Thread>();


        /// <summary>
        /// The type of this server <see cref="ServerTypes"/>
        /// </summary>
        private ServerTypes serverType { get; set; } = ServerTypes.SERVER_UNKNOWN;



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
        /// <param name="server_type">The type of server</param>
        /// <param name="address">The address for this server</param>
        /// <param name="port">The port for this server</param>
        public ServerModel(ServerTypes server_type, string address, Int32 port) : base(null, null)
        {
            Type = server_type;
            Address = address;
            Port = port;

            Console.WriteLine("-- Creating server of type " + server_type + " on " + address + ": " + port);

            LaunchUpdateServerThread();
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// The thread callback routine for this server
        /// </summary>
        public virtual void HandleServerThread() { }

        /// <summary>
        /// Start the server and start listening for connections
        /// </summary>
        public virtual void StartServer() 
        {
            IsRunning = true;
            Console.WriteLine("------ Starting server #" + ID.ToString());
        }
        public virtual void Update()
        {
//            Console.WriteLine("------- Checking for server updates on server " + ID.ToString());
        }

        #endregion

        #region Public Methods
        public void LaunchUpdateServerThread()
        {
            Console.WriteLine("------- Launching server " + ID + " update thread");
            UpdateThread = new Thread(RunUpdateThread);
            UpdateThread.Start();
        }


        /// <summary>
        /// Adds a client model to this servers partipant list
        /// </summary>
        /// <param name="clientModel"></param>
        public void AddClientModelToServer(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            participantClientList.Add(clientModel.ID, clientModel);
        }

        /// <summary>
        /// Remove the client from the server
        /// </summary>
        /// <param name="clientModel"></param>
        public void RemoveClientModelFromServer(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            if(participantClientList.ContainsKey(clientModel.ID))
                participantClientList.Remove(clientModel.ID);
        }



        #endregion




        /// <summary>
        /// A function for shutting down the server.
        /// </summary>
        public void ServerShutdown()
        {
            //// Shutdown and end connection         
            ListenerSocket.Stop();
            Console.WriteLine(" >> " + "exit");
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

        private void RunUpdateThread()
        {
            Console.WriteLine("- Update thread for server " + ID + " created");
            while (!ShouldShutdown)
            {
                Console.WriteLine("Updating server " + ID);
                this.Update();
                Thread.Sleep(2000);

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
    //    // The server thart created this client thread
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

