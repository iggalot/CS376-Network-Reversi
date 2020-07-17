using System;
using System.Net.Sockets;
using System.Threading;

namespace ClientServerLibrary
{
    public class DataTransmission
    {
        /// <summary>
        /// The number of attempts to successful send a packet
        /// </summary>
        public const int MAX_SEND_ATTEMPTS = 5;

        /// <summary>
        /// Was the transmission deemed a success?
        /// </summary>
        public static bool SendSuccess = true;

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

            // Retrieve the steam for the client socket
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
                    break;
            }

            return SendSuccess;
        }
    }
}
