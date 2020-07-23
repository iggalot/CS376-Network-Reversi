using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    /// <summary>
    /// The basic packet class for information to be shared between clients and the server 
    /// </summary>
    public class PacketInfo
    {
        public int Id;
        public string Data;
        public PacketType Type;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">ID of the sender of the packet</param>
        /// <param name="data">The string message to send</param>
        /// <param name="type">The type of the connection</param>
        public PacketInfo(int id, string data, PacketType type)
        {
            Id = id;
            Data = data;
            Type = type;
        }

        public PacketInfo()
        {
            Id = -1;
            Data = String.Empty;
            Type = PacketType.PACKET_UNDEFINED;
        }

        /// <summary>
        /// Forms the packet information into a string to be transmitted
        /// </summary>
        /// <param name="id">The id of the sender</param>
        /// <param name="data">The string data to send</param>
        /// <param name="type">The type of packet being sent</param>
        /// <returns></returns>
        public string FormPacket()
        {
            return (Type.ToString() + "###" + Id.ToString() + "###" + Data + "###" + "$");
        }

        public void UnpackPacket(string data)
        {
            string[] separateStrings = { "###" }; // The packet data separators
            string[] words = data.Split(separateStrings, StringSplitOptions.None);

            // packet:  type ### id ### data
            this.Type = (PacketType)Enum.Parse(typeof(PacketType), words[0]);
            this.Id = Int32.Parse(words[1]);
            this.Data = words[2];
        }
    }

    /// <summary>
    /// Enum for types of packets that the client and servers will use
    /// This is used as a STATE machine for the server and client.
    /// TODO:  Should this be here?  or in the game specific enums?  PacketInfo in this library requires these types...
    /// </summary>
    public enum PacketType
    {
        PACKET_UNDEFINED =             -1,  // An undefined packet type
        PACKET_CONNECTION_REQUEST =     0,
        PACKET_CONNECTION_ACCEPTED =    1,
        PACKET_CONNECTION_REFUSED =     2,
        PACKET_GAME_STARTING =          3,
        PACKET_GAMEMOVE_REQUEST =       4,
        PACKET_GAMEMOVE_ACCEPTED =      5,  // if a move is deemed as valid
        PACKET_GAMEMOVE_DENIED =        6,  // if a move is deemed invalid
        PACKET_SWITCH_PLAYERS =         7,  // Switches the current player and the opponent  TODO:  Should this be here? Or is this game specific?
        PACKET_GAME_ENDING =            8
    }
}
