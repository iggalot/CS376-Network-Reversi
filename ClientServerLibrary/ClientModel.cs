using System;
using System.Net.Sockets;
using System.Text;

namespace ClientServerLibrary
{
    /// <summary>
    /// A generic client class utilizing the TcpClient protocol.  Requires a TcpListener 
    /// on the other end to create the socket before communications can start.
    /// </summary>
    
    [Serializable]
    public class ClientModel : ClientServerInfoModel, IConnectionHandler
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientModel() : base(null,null)
        {

        }

        #region Public Methods
        ///// <summary>
        ///// Requests an id from its related server
        ///// </summary>
        ///// <returns></returns>
        //public int RequestID()
        //{
        //    // TODO:  return a new ID from the server or servermanager.
        //    return 0;
        //}

        #endregion

        #region IConnectionHandler Interface Implementation
        public void AcceptConnection()
        {
            throw new NotImplementedException();
        }
        public void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public TcpClient ListenForConnections()
        {
            throw new NotImplementedException();
        }

        public void MakeConnection()
        {
            throw new NotImplementedException();
        }

        public void RefuseConnection()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}

