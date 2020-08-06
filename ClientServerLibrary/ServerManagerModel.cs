using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientServerLibrary
{
    public class ServerManagerModel : ServerModel, IConnectionHandler
    {
        #region Private Properties

        /// <summary>
        /// The list of servers
        /// </summary>
        private Dictionary<int, ServerModel> serverList = new Dictionary<int, ServerModel>();

        #endregion

        #region Public Properties


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
        public void AddClientModelToConnectionList(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            if (ConnectedClientModelList.ContainsKey(clientModel.ID) == true)
            {
                Console.WriteLine("This client ID already exists.");
                return;
            }
                
            ConnectedClientModelList.Add(clientModel.ID, clientModel);

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
        public TcpListener StartManager()
        {
            // Convert the string address to an IPAddress type
            IPAddress localAddr = IPAddress.Parse(Address);

            // TcpListener -- create the server socket
            this.ListenerSocket = new TcpListener(localAddr, Port);

            // Start listening for connections
            this.ListenerSocket.Start();

            return ListenerSocket;

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

        /// <summary>
        /// Routine to shut down the manager.  First shuts down all servers controlled by this manager
        /// and then finally shuts down itself.
        /// </summary>
        public override void Shutdown()
        {
            // First shut down all running servers...
            foreach(KeyValuePair<int, ServerModel> server in serverList)
            {
                server.Value.Shutdown();
            }

            // And then shuts down itself.
            base.Shutdown();
        }

        /// <summary>
        /// Display the connected clients information
        /// </summary>
        /// <returns></returns>
        public string ListConnectedClients()
        {
            string str = string.Empty;
            str += " ---------------------------------------------------\n";
            foreach(KeyValuePair<int,ClientModel> item in ConnectedClientModelList)
            {
                str += item.Key.ToString() + " --- " + item.Value.ListInfo() + "\n";
            }
            str += " ---------------------------------------------------\n";

            return str;
        }
        #endregion

        #region IConnectionHandler Interface Implementation
        /// <summary>
        /// Disconnects the client and associated network stream
        /// </summary>
        public override void CloseConnection()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Listen for Connections once the server has started
        /// </summary>
        /// <returns></returns>
        public override TcpClient ListenForConnections()
        {
            Console.WriteLine("Waiting for new connections...");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            ConnectionSocket = ListenerSocket.AcceptTcpClient();

            Console.WriteLine(" ==== New client connected on ServerManager " + ID);

            return ConnectionSocket;
        }


        #endregion
    }
}
