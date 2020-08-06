using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{

    public interface IConnectionHandler
    {
        void AcceptConnection(ClientModel client);
        void RefuseConnection(ClientModel client);

        void AcceptOrRefuseConnection(ClientModel client, out ConnectionStatusTypes status);

        TcpClient MakeConnection(string v, Int32 port);
        void CloseConnection();
        TcpClient ListenForConnections();

    }
}
