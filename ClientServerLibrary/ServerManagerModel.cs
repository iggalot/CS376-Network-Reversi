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
        private Dictionary<int, ServerModel> _serverList = new Dictionary<int, ServerModel>();

        #endregion

        #region Public Properties

        public Dictionary<int, ServerModel> ServerList 
        {
            get => _serverList; 
            set
            {
                if (value == null)
                    return;

                _serverList = value;
            }
        }

        #endregion

        #region Constructor

        public ServerManagerModel(ServerTypes serverType, string address, Int32 port) : base(serverType, address, port)
        {
            Console.WriteLine("---- Creating a new Server Manager with ID:" + Id.ToString());
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a client socket to the connected sockets list
        /// </summary>
        /// <param name="clientModel">The client model</param>
        public void AddClientModelToConnectionList(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            if (ConnectedClientModelList.ContainsKey(clientModel.Id) == true)
            {
                Console.WriteLine("This client ID already exists.");
                return;
            }
                
            ConnectedClientModelList.Add(clientModel.Id, clientModel);

        }
        /// <summary>
        /// Adds a server to the server list
        /// </summary>
        /// <param name="server"></param>
        public void AddServerToManager(ServerModel server)
        {
            if (server == null)
                return;

            if (_serverList.ContainsKey(server.Id) == false)
            {
                Console.WriteLine("----- Adding server to manager...");
                _serverList.Add(server.Id, server);
            }
        }

        /// <summary>
        /// Tries to retrieve a specific server from the server list.
        /// Returns the model if true, otherwise returns a null object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServerModel GetServerFromListById(int id)
        {
            if(_serverList.TryGetValue(id, out var model))
                return model;
            return null;
        }

        public virtual ServerModel GetAvailableServer(ServerTypes serverType) { return null; }


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
            IPAddress localAddr;
            localAddr = IPAddress.Parse(Address);

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
            Console.WriteLine("(ServerManagerModel:) HandleServerThread");
            //base.HandleServerThread();

        }

        #endregion

        #region Virtual Overrides

        /// <summary>
        /// Run the manager and start the listening process
        /// </summary>
        public override void StartServer()
        {
            Console.WriteLine(" >> (ServerManagerModel:) Server Manager started at " + Address + " on port " + Port.ToString());
            base.StartServer();
        }

        #endregion

        #region Private Methods


        public void RemoveClientModelFromConnectedList(ClientModel clientModel)
        {
            if (clientModel == null)
                return;

            ConnectedClientModelList.Remove(clientModel.Id);
        }



        private void DeleteServer(ServerModel server)
        {
            if (server == null)
                return;

            if(_serverList.ContainsKey(server.Id) == true)
            {
                Console.WriteLine("----- Removing server from manager...");
                _serverList.Remove(server.Id);
            }

        }

        /// <summary>
        /// Routine to shut down the manager.  First shuts down all servers controlled by this manager
        /// and then finally shuts down itself.
        /// </summary>
        public override void Shutdown()
        {
            // First shut down all running servers...
            foreach(KeyValuePair<int, ServerModel> server in _serverList)
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

            Console.WriteLine(" ==== New client connected on ServerManager " + Id);

            return ConnectionSocket;
        }


        #endregion
    }
}
