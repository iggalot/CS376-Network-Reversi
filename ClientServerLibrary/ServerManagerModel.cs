using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    public class ServerManagerModel : ServerModel, IConnectionHandler
    {
        #region Private Properties

        /// <summary>
        /// The list of servers
        /// </summary>
        private Dictionary<int, ServerModel> serverList = new Dictionary<int, ServerModel>();

        /// <summary>
        /// The list of connected clients
        /// </summary>
        private Dictionary<int, TcpClient> connectedClientSocketList = new Dictionary<int, TcpClient>();

        #endregion

        #region Public Properties

        public Dictionary<int, TcpClient> ConnectedClientSocketList
        {
            get => connectedClientSocketList;
        }
        #endregion

        #region Constructor

        public ServerManagerModel(ServerTypes server_type, string address, Int32 port) : base(server_type, address, port)
        {
            Console.WriteLine("---- Creating a new Server Manager with ID:" + ID.ToString());
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a client socket to the connected sockets list
        /// </summary>
        /// <param name="clientSocket"></param>
        public void AddClientSocketToConnectionList(TcpClient clientSocket)
        {
            if (clientSocket == null)
                return;

            //connectedClientSocketList.Add(clientSocket);
        }
        /// <summary>
        /// Adds a server to the server list
        /// </summary>
        /// <param name="server"></param>
        public void AddServer(ServerModel server)
        {
            if (server == null)
                return;

            if (serverList.ContainsKey(server.ID) == false)
            {
                Console.WriteLine("----- Adding server to manager...");
                serverList.Add(server.ID, server);
            }
        }

        /// <summary>
        /// Tries to retrieve a specific server from the server list.
        /// Returns the model if true, otherwise returns a null object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServerModel GetServerByID(int id)
        {
            ServerModel model;
            if(serverList.TryGetValue(id, out model))
                return model;
            return null;
        }

        /// <summary>
        /// Configure any options for the server manager here
        /// </summary>
        public void ConfigureManager()
        {

        }

        /// <summary>
        /// Starts the server manager listening for connections
        /// </summary>
        public void StartManager()
        {
            // Convert the string address to an IPAddress type
            IPAddress localAddr = IPAddress.Parse(Address);

            // TcpListener -- create the server socket
            ListenerSocket = new TcpListener(localAddr, Port);

            // Start listening for connections
            ListenerSocket.Start();
        }

        /// <summary>
        /// The thread for the server object's primary responsibility
        /// </summary>
        public override void HandleServerThread()
        {
            base.HandleServerThread();

        }

        #endregion

        #region Virtual Overrides

        /// <summary>
        /// Run the manager and start the listening process
        /// </summary>
        public override void StartServer()
        {
            base.StartServer();

            Console.WriteLine(" >> Server Manager started at " + Address + " on port " + Port.ToString());


       }

        #endregion

        #region Private Methods
        private void AddClientToConnectedList(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            //if (connectedClientModelList.ContainsKey(clientModel.ID) == false)
            //    connectedClientModelList.Add(clientModel.ID, clientModel);
        }

        private void RemoveClientFromConnectedList(ClientModel clientModel)
        {
            throw new NotImplementedException();
        }



        private void DeleteServer(ServerModel server)
        {
            if (server == null)
                return;

            if(serverList.ContainsKey(server.ID) == true)
            {
                Console.WriteLine("----- Removing server from manager...");
                serverList.Remove(server.ID);
            }

        }

        private void ShutdownServer(int id)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IConnectionHandler Interface Implementation
        public void MakeConnection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects the client and associated network stream
        /// </summary>
        /// <param name="stream">The network stream to be used</param>
        /// <param name="client">The client that should be disconnected.</param>
        public void CloseConnection(NetworkStream stream, TcpClient client)
        {
            stream.Close();
            client.Close();
        }

        public void AcceptConnection()
        {
            throw new NotImplementedException();
        }

        public void RefuseConnection()
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Listen for Connections once the server has started
        /// </summary>
        /// <returns></returns>
        public TcpClient ListenForConnections()
        {
            Console.WriteLine("Waiting for new connections...");

            // Create a default client socket to be used by each thread.
            TcpClient clientSocket = default(TcpClient);

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            clientSocket = ListenerSocket.AcceptTcpClient();

            return clientSocket;
        }

        #endregion
    }
}
