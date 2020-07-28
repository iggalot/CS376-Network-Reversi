using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientServerLibrary
{
    /// <summary>
    /// Our primary server class that handles multiple client connections
    /// </summary>
    [Serializable]
    public class Server : DataTransmission
    {
        // Store our list of client sockets.
        public static Hashtable ClientSocketList { get; set; }

        // The socket for our server
        public static TcpListener ServerSocket { get; set; }

        // The current data held by the server
        public string TempData { get; set; }

        // The current status connection result
        public PacketType TempStatus { get; set; }

        // Tells our server that it should shutdown
        public static bool ShouldShutdownNow { get; set; } = false;

        public const string RejectConnectionMessage = "The server has rejected your connection. Please try again later.";
        public const string AcceptConnectionMessage = "Welcome to the Reversi / Othello server!";

        /// <summary>
        /// Default constructor
        /// </summary>
        public Server() { }

        /// <summary>
        /// Creates a server instance at a specified address and port.
        /// </summary>
        /// <param name="address">The address of the server</param>
        /// <param name="port">The port of the server</param>
        public Server(string address, Int32 port)
        {
            // create the list to store all of our connected sockets
            ClientSocketList = new Hashtable();

            // Convert the string address to an IPAddress type
            IPAddress localAddr = IPAddress.Parse(address);

            // TcpListener -- create the server socket
            ServerSocket = new TcpListener(localAddr, port);

            // Start listening for client requests.
            ServerSocket.Start();
            Console.WriteLine(" >> " + "Server Started at " + address + " on port " + port.ToString());


            //// Now listen for connections.
            //ListenForConnections();
        }

        /// <summary>
        /// Listen for Connections
        /// </summary>
        /// <returns></returns>
        public TcpClient ListenForConnections()
        {
            // Create a default client socket to be used by each thread.
            TcpClient clientSocket = default(TcpClient);

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            clientSocket = ServerSocket.AcceptTcpClient();

            return clientSocket;
        }

        /// <summary>
        /// Converts data received back into a useable packet
        /// </summary>
        /// <returns></returns>
        //public PacketInfo ReceiveData(string data)
        //{
        //    string[] separateStrings = { "###" }; // The packet data separators
        //    string[] words = data.Split(separateStrings,StringSplitOptions.None);

        //    // packet:  type ### id ### data
        //    PacketType type = (PacketType)Enum.Parse(typeof(PacketType), words[0]);
        //    int id = Int32.Parse(words[1]);
        //    string name = words[2];

        //    // recreate the packet info
        //    PacketInfo packet = new PacketInfo(id, name, type);
        //    return packet;
        //}

        /// <summary>
        /// A function for shutting down the server.
        /// </summary>
        public void Shutdown()
        {
            //// Shutdown and end connection         
            ServerSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }

        /// <summary>
        /// A server routine to broadcast a string message to all clients currently connected to
        /// the server.
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastToAll(string message, string uName, bool flag)
        {
            foreach(DictionaryEntry client in ClientSocketList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)client.Value;

                if (broadcastSocket == null || !broadcastSocket.Connected)
                {
                    ClientSocketList.Remove(client);
                    continue;
                }

                try
                {
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (flag == true)
                    {
                        broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + message);
                    }
                    else
                    {
                        broadcastBytes = Encoding.ASCII.GetBytes(message);
                    }

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                } 
                catch (System.IO.IOException e)
                {
                    // If our socket has disconnected, remove the client
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Class to handle each client request separately.
    /// </summary>
    public class handleClient
    {
        TcpClient clientSocket;
        // The server thart created this client thread
        Hashtable clientsList;
        string clientNum;

        /// <summary>
        /// The function to start the client thread.
        /// </summary>
        /// <param name="inClientSocket">The associated client socket</param>
        /// <param name="strClientNum">String containing the client's number</param>
        public void startClient(TcpClient inClientSocket, string strClientNum, Hashtable clist)
        {
            this.clientSocket = inClientSocket;
            this.clientNum = strClientNum;
            this.clientsList = clist;
            Thread ctThread = new Thread(()=>doChat());
            ctThread.Start();
        }

        /// <summary>
        /// The main thread function for each connected client.
        /// </summary>
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[65536];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                if (!clientSocket.Connected)
                    break;

                try
                {
                    // increment our response counter
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client - " + clientNum + ": " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    Server.BroadcastToAll(dataFromClient, clientNum, true);
                }

                catch (System.IO.IOException ex)
                {
                    Console.WriteLine(" >> Client #" + clientNum + " has disconnected.");
                    // Close the socket
                    clientSocket.Close();
                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                    break;
                }
            }
        }
    }
}

