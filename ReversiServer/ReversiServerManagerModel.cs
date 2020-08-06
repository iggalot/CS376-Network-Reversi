using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
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
            // Check for enough clients to play the game
        }
    }
}
