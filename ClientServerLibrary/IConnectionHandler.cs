using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{

    public interface IConnectionHandler
    {
        void AcceptConnection(ClientServerInfoModel client);
        void RefuseConnection(ClientServerInfoModel client);

        void AcceptOrRefuseConnection(ClientServerInfoModel client, out ConnectionStatusTypes status);

        void MakeConnection();
        void CloseConnection();
        TcpClient ListenForConnections();

    }
}
