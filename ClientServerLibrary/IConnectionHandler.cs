using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{

    public interface IConnectionHandler
    {
        TcpClient MakeConnection(string v, Int32 port);
        void CloseConnection();
        TcpClient ListenForConnections();

    }
}
