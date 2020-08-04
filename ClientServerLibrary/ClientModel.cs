using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    /// <summary>
    /// A generic client class utilizing the TcpClient protocol.  Requires a TcpListener 
    /// on the other end to create the socket before communications can start.
    /// </summary>

    [Serializable]
    public class ClientModel : ClientServerInfoModel, IConnectionHandler
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientModel() : base(null,null)
        {

        }

        /// <summary>
        /// Creates a client model for a client socket
        /// </summary>
        /// <param name="client">The client socket</param>
        public ClientModel(TcpClient client, TcpListener listener) : base(client, listener)
        {

        }
        #endregion



        #region IConnectionHandler Interface Implementation
        public override void AcceptConnection(ClientServerInfoModel model)
        {
            // Do nothing since a client shouldnt have this functionality
        }
        public override void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public override TcpClient ListenForConnections()
        {
            throw new NotImplementedException();
        }

        public override void MakeConnection()
        {
            throw new NotImplementedException();
        }

        public override void RefuseConnection(ClientServerInfoModel model)
        {
            // Do nothing since a client shouldnt have this functionality
        }

        public override void AcceptOrRefuseConnection(ClientServerInfoModel model, out ConnectionStatusTypes status)
        {
            // Do nothing since a client shouldnt have this functionality
            status = ConnectionStatusTypes.STATUS_CONNECTION_INCAPABLE;
        }
        #endregion

    }
}

