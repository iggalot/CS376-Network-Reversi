using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace ReversiServer
{
    public class ReversiServerManagerModel : ServerManagerModel
    {
        public ReversiServerManagerModel() : base(ServerTypes.SERVER_SERVERMANAGER,GlobalSettings.ServerAddress,GlobalSettings.Port_GameServer)
        {

        }

        /// <summary>
        /// Accepts a connection request
        /// </summary>
        /// <param name="new_model"></param>
        public ReversiClientModel AcceptConnection(ReversiClientModel new_model)
        {

            // And immediately return it without updating the ID
            new_model.CurrentStatus = ConnectionStatusTypes.STATUS_CONNECTION_ACCEPTED;

            try
            {
                DataTransmission.SerializeData<ReversiClientModel>(new_model, new_model.ConnectionSocket);
                Console.WriteLine("... GameServer: Connection accepted for client #" + new_model.ID);
            }
            catch
            {
                new_model.CurrentStatus = ConnectionStatusTypes.STATUS_CONNECTION_ERROR;
            }

            return new_model;
        }

        /// <summary>
        /// Refuse a connection and close the socket
        /// </summary>
        /// <param name="new_model"></param>
        public ReversiClientModel RefuseConnection(ReversiClientModel new_model)
        {
            // And immediately return it without updating the ID
            new_model.CurrentStatus = ConnectionStatusTypes.STATUS_CONNECTION_REFUSED;

            try
            {
                DataTransmission.SerializeData<ReversiClientModel>(new_model, new_model.ConnectionSocket);
            }
            catch
            {
                throw new SocketException();
            }

            new_model.ConnectionSocket.Close();
            return new_model;
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

            if ((model == null) || (model.ConnectionSocket == null))
            {
                status = ConnectionStatusTypes.STATUS_CONNECTION_ERROR;
                return null;
            }

            int timeoutCount = 0;
            while ((timeoutCount > ServerSettings.MaxServerTimeoutCount) || (!model.ConnectionSocket.GetStream().DataAvailable))
            {
                Thread.Sleep(200);
                timeoutCount++;
            }

            if (timeoutCount > ServerSettings.MaxServerTimeoutCount)
            {
                Console.WriteLine("Connection has timedout");
                status = ConnectionStatusTypes.STATUS_CONNECTION_ERROR;
                return null;
            }

            // Send a connection declined message for too many connections
            if (ConnectedClientModelList.Count > ServerSettings.MaxServerConnections)
            {
                model = RefuseConnection(model);
                status = ConnectionStatusTypes.STATUS_CONNECTION_REFUSED;
                model.CurrentStatus = status;
            }
            // Otherwise accept the connection
            else
            {
                model = AcceptConnection(model);
                status = ConnectionStatusTypes.STATUS_CONNECTION_ACCEPTED;
                model.CurrentStatus = status;
            }

            return model;
        }





        /// <summary>
        /// The main thread handle for this reversi server.
        /// </summary>
        public override void HandleServerThread()
        {
            Console.WriteLine("Server " + ID + ":  Starting HandleServerThread");

            while(!ShouldShutdown)
            {
                if (ConnectedClientModelList.Count == 0)
                {
                    Console.WriteLine("No clients on ReversiServerManager " + ID);
                    Thread.Sleep(ServerSettings.ServerUpdatePulseDelay);
                    continue;
                } else
                {
                    // Check for enough clients to place on a server
                    Console.WriteLine("RSM currently has " + ConnectedClientModelList + " connected to it...");
                }

                // Get the oldest client
                ReversiClientModel oldest_client_model = (ReversiClientModel)GetOldestClientModelFromConnectedList();
                Console.WriteLine("-- Oldest available client is: " + oldest_client_model.ID);

                // Find an available game server or create a new one
                ReversiGameServerModel availableServer = (ReversiGameServerModel)GetAvailableServer(ServerTypes.SERVER_GAMESERVER);
                Console.WriteLine("-- Available server is: " + availableServer.ID);

                // Move the client model to a reversi game server....
                availableServer.AddClientModelToServer(oldest_client_model);
                Console.WriteLine("-- Adding client " + oldest_client_model.ID + " to game server " + availableServer.ID);

                // And remove it from the connected list
                RemoveClientModelFromConnectedList(oldest_client_model);
                Console.WriteLine("-- Client removed from ReversiServerManager list");

            }
        }

        /// <summary>
        /// Function that finds a server with space for additional clients to be added.
        /// </summary>
        /// <param name="server_type">The type of server to search for</param>
        /// <returns></returns>
        public override ServerModel GetAvailableServer(ServerTypes server_type)
        {
            // Is there a server with room on it?
            bool available = false;
            int available_server_index = -1;
            foreach (KeyValuePair<int, ServerModel> server in ServerList)
            {
                if (server.Value.ConnectedClientModelList.Count < ReversiSettings.MaxReversiServerConnections - 1)
                {
                    // Make sure it's a game server
                    if (server.Value.Type == server_type)
                    {
                        available = true;
                        available_server_index = server.Key;
                        break;
                    }
                }
            }

            // If no servers are available, create a new server;
            ServerModel new_server;
            if (!available)
            {
                // TODO:  Will this crash if there is alreadt a reversi game server running?
                new_server = new ReversiGameServerModel(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
                AddServerToManager(new_server);
            }
            else
            {
                // Retrieve the server by its index value
                new_server = GetServerFromListByID(available_server_index);
            }

            return new_server;
        }

    }
}
