using ClientServerLibrary;
using Reversi.Models;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Reversi;
using Settings;

namespace ReversiServer
{
    public class ReversiServerManagerModel : ServerManagerModel
    {
        public ReversiServerManagerModel() : base(ServerTypes.ServerServermanager,GlobalSettings.ServerAddress,GlobalSettings.Port_GameServer)
        {

        }

        /// <summary>
        /// Accepts a connection request
        /// </summary>
        /// <param name="newModel"></param>
        public ReversiClientModel AcceptConnection(ReversiClientModel newModel)
        {

            // And updating the ID and status
            newModel.CurrentStatus = ConnectionStatusTypes.StatusConnectionAccepted;
            newModel.ClientPlayer.PlayerId = newModel.NextId();   // updates the players ID here.
            try
            {
                DataTransmission.SerializeData<ReversiClientModel>(newModel, newModel.ConnectionSocket);
                Console.WriteLine("... GameServer: Connection accepted for client #" + newModel.Id);
            }
            catch
            {
                newModel.CurrentStatus = ConnectionStatusTypes.StatusConnectionError;
            }

            return newModel;
        }

        /// <summary>
        /// Refuse a connection and close the socket
        /// </summary>
        /// <param name="newModel"></param>
        public ReversiClientModel RefuseConnection(ReversiClientModel newModel)
        {
            // And immediately return it without updating the ID
            newModel.CurrentStatus = ConnectionStatusTypes.StatusConnectionRefused;

            try
            {
                DataTransmission.SerializeData<ReversiClientModel>(newModel, newModel.ConnectionSocket);
            }
            catch
            {
                throw new SocketException();
            }

            newModel.ConnectionSocket.Close();
            return newModel;
        }


        /// <summary>
        /// Determines whether a connection should be accepted or refused
        /// </summary>
        /// <param name="model">The model to accept or refuse</param>
        /// <param name="status">The returned status of this connection request</param>
        public ReversiClientModel AcceptOrRefuseConnection(ReversiClientModel model, out ConnectionStatusTypes status)
        {
            string str = model.ConnectionSocket.Client.Handle.ToString();
            Console.WriteLine(str);

            if ((model.ConnectionSocket == null))
            {
                status = ConnectionStatusTypes.StatusConnectionError;
                return null;
            }

            // Send a connection declined message for too many connections
            if (ConnectedClientModelList.Count > ServerSettings.MaxServerConnections)
            {
                model = RefuseConnection(model);
                status = ConnectionStatusTypes.StatusConnectionRefused;
            }
            // Otherwise accept the connection
            else
            {
                model = AcceptConnection(model);
                status = ConnectionStatusTypes.StatusConnectionAccepted;
            }

            model.CurrentStatus = status;

            return model;
        }



        /// <summary>
        /// The main thread handle for this reversi server.
        /// </summary>
        public override void HandleServerThread()
        {
            Console.WriteLine("Server " + Id + ":  Starting HandleServerThread");

            while(!ShouldShutdown)
            {
                if (ConnectedClientModelList.Count == 0)
                {
                    Console.WriteLine("No clients on ReversiServerManager " + Id);
                    Thread.Sleep(ServerSettings.ServerUpdatePulseDelay);
                    continue;
                }

                // Check for enough clients to place on a server
                Console.WriteLine("RSM currently has " + ConnectedClientModelList.Count + " clients connected to it...");

                // Get the oldest client
                ReversiClientModel oldestClientModel = (ReversiClientModel)GetOldestClientModelFromConnectedList();
                Console.WriteLine("-- Oldest available client is: " + oldestClientModel.Id);

                // Find an available game server or create a new one
                ReversiGameServerModel availableServer = (ReversiGameServerModel)GetAvailableServer(ServerTypes.ServerGameserver);
                Console.WriteLine("-- Available server is: " + availableServer.Id);

                // Move the client model to a reversi game server....
                availableServer.AddClientModelToServer(oldestClientModel);
                Console.WriteLine("-- Adding client " + oldestClientModel.Id + " to game server " + availableServer.Id);

                // And remove it from the connected list
                RemoveClientModelFromConnectedList(oldestClientModel);
                Console.WriteLine("-- Client removed from ReversiServerManager list");
            }
        }

        /// <summary>
        /// Function that finds a server with space for additional clients to be added.
        /// </summary>
        /// <param name="serverType">The type of server to search for</param>
        /// <returns></returns>
        public override ServerModel GetAvailableServer(ServerTypes serverType)
        {
            // Is there a server with room on it?
            var available = false;
            var availableServerIndex = -1;
            foreach (var server in ServerList
                .Where(server => server.Value.ConnectedClientModelList.Count < ReversiSettings.MaxReversiServerConnections - 1)
                .Where(server => server.Value.Type == serverType))
            {
                available = true;
                availableServerIndex = server.Key;
                break;
            }

            // If no servers are available, create a new server;
            ServerModel newServer;
            if (!available)
            {
                // TODO:  Will this crash if there is already a reversi game server running?
                newServer = new ReversiGameServerModel(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
                AddServerToManager(newServer);
            }
            else
            {
                // Retrieve the server by its index value
                newServer = GetServerFromListById(availableServerIndex);
            }

            return newServer;
        }

    }
}
