using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ClientServerLibrary
{

    public class DataTransmission
    {
        #region Public Properties

        #endregion

        /// <summary>
        /// Tells the server it should shutdown
        /// </summary>
        public bool ShouldShutdown { get; set; } = false;

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
        /// The max size of the read / write socket data array
        /// </summary>
        public const int MAX_DATA_SIZE = 1024;

        /// <summary>
        /// The number of attempts to successful send a packet
        /// </summary>
        public const int MAX_SEND_ATTEMPTS = 5;

        /// <summary>
        /// Was the transmission deemed a success?
        /// </summary>
        public static bool SendSuccess = true;

        /// <summary>
        /// Receives data from a socket.
        /// </summary>
        /// <param name="socket">The socket to receive data from</param>
        /// <param name="packet">The packet <see cref="PacketInfo"/> of information to create when receiving data</param>
        /// <returns></returns>
        public static bool ReceiveData(TcpClient socket, out PacketInfo packet)
        {
            PacketInfo newpacket = new PacketInfo();
            StringBuilder message = new StringBuilder();
            bool dataRead = false;

            if (!socket.Connected)
            {
                throw new SocketException();
            }

            // Get the stream associated with the socket.
            NetworkStream stream = socket.GetStream();
            packet = null;

            while (stream.CanRead)
            {
                if (!stream.DataAvailable)
                {
        //            Console.WriteLine("ReceiveData: There was nothing in the stream at this time.");
                }
                else
                {
                    // Receive the TcpServer response.
                    // Buffer to store the response bytes.
                    Byte[] receivedata = new byte[MAX_DATA_SIZE];

                    int numberOfBytesRead = 0;

                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        numberOfBytesRead = stream.Read(receivedata, 0, receivedata.Length);
                        message.AppendFormat("{0}", Encoding.ASCII.GetString(receivedata, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine("Data Received: {0}", message);
                    dataRead = true;

                    if (dataRead)
                    {
                        stream.Flush();

                        // Reform the basic packet information.
                        newpacket.UnpackPacket(message.ToString());

                        // Assign the packet to the outgoing variable.
                        packet = newpacket;
                        break;
                    }
                }
            }

            return dataRead;
        }

        /// <summary>
        /// Function that sends a packet of information to a tcpclient
        /// </summary>
        /// <param name="client">The target socket</param>
        /// <param name="packet">The <see cref="PacketInfo"/> packet to send</param>
        public static bool SendData(TcpClient client, PacketInfo packet)
        {
            // Reset the SendSuccess flag
            SendSuccess = false;

            // Check if the client socket is still connected...
            if (!client.Connected)
                return false;

            // Retrieve the stream for the client socket
            NetworkStream serverStream = client.GetStream();

            int attempt = 0;
            while (attempt < MAX_SEND_ATTEMPTS)
            {
                try
                {
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(packet.FormPacket());
                    IAsyncResult r = serverStream.BeginWrite(outStream, 0, outStream.Length, null, null);
                    r.AsyncWaitHandle.WaitOne(1000);
                    serverStream.EndWrite(r);
                    serverStream.Flush();
                    SendSuccess = true;
                } catch
                {
                    ///Thread.Sleep(1000);
                    Console.WriteLine("... Failed to send packet (attempt #" + attempt + ")");
                    attempt++;
                }

                // Exit out of our send loop attempt
                if (SendSuccess)
                {
                    Console.WriteLine("... Data Sent: " + packet.FormPacket());
                    break;
                }
            }

            return SendSuccess;
        }

        /// <summary>
        ///  Send packet to list of multiple players
        /// </summary>
        /// <param name="list">List of client sockets (playerS)</param>
        /// <param name="packet">The packet to broadcast</param>
        /// <returns></returns>
        public static bool SendDataToMultipleUsers(List<TcpClient> list, PacketInfo packet)
        {
            bool status = true;

            foreach(TcpClient client in list)
            {
                bool send_status = false;

                // Check that the client is connected
                if(client.Connected)
                {
                    send_status = SendData(client, packet);
                }

                if (send_status == false)
                    status = false;
            }

            return status;
        }

        /// <summary>
        /// Flushes the sockets for multiple users.  Useful for process milestones such as
        /// starting a game or ending a game, prior to broadcasting.
        /// </summary>
        /// <param name="list"></param>
        public static void FlushMultipleUsers(List<TcpClient> list)
        {
            foreach(TcpClient item in list)
            {
                item.GetStream().Flush();
            }
        }

        /// <summary>
        /// Method that takes generic data of type T and a data stream and serializes it for transmission
        /// across the socket.
        /// </summary>
        /// <typeparam name="T">The generic class of the data being serializes</typeparam>
        /// <param name="data">The data to be serialized</param>
        /// <param name="stream">The stream to send the serialized data to (FileStream, NetworkStream, etc.) </param>
        public static void SerializeData<T>(T data, TcpClient client) where T : class
        {
            if (!client.Connected)
            {
                throw new SocketException();
            }

            NetworkStream stream = client.GetStream();

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            //stream.Close();
        }

        /// <summary>
        /// Method that deserializes the data on a network stream
        /// </summary>
        /// <typeparam name="T">The generic class of the data that was serializes</typeparam>
        /// <param name="stream">The stream in which to deserialize (FileStream, NetworkStream, etc.)</param>
        /// <returns></returns>
        public static T DeserializeData<T>(TcpClient client) where T: class
        {
            if(!client.Connected)
            {
                throw new SocketException();
            }
            NetworkStream stream = client.GetStream();

            IFormatter formatter = new BinaryFormatter();
            T objnew = (T)formatter.Deserialize(stream);
            return objnew;
        }

        public static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }


    }
}
