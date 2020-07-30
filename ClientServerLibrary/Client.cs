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
    public class Client : DataTransmission
    {
        /// <summary>
        /// Connects our client to a TcpIp socket
        /// </summary>
        /// <param name="v">Address to connect to</param>
        /// <param name="port">Port number to connect to</param>
        /// <returns></returns>
        public static TcpClient Connect(string v, Int32 port)
        {
            return new TcpClient(v, port);
        }

        /// <summary>
        /// Disconnects the client and associated network stream
        /// </summary>
        /// <param name="stream">The network stream to be used</param>
        /// <param name="client">The client that should be disconnected.</param>
        public static void Disconnect(NetworkStream stream, TcpClient client)
        {
            stream.Close();
            client.Close();
        }
    }
}

