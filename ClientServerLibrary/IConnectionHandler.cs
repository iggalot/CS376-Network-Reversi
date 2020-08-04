using System.Net.Sockets;

namespace ClientServerLibrary
{
    public interface IConnectionHandler
    {

        void AcceptConnection();
        void RefuseConnection();
        void MakeConnection();
        void CloseConnection();
        TcpClient ListenForConnections();

    }
}
