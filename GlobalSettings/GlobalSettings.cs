using System;

namespace Settings
{
    public static class GlobalSettings
    {
        /// <summary>
        /// The address of our server
        /// </summary>
        public static string ServerAddress { get; set; } = "127.0.0.1";

        /// <summary>
        ///  The port that the game server and server manager will be listening on
        /// </summary>
        public static Int32 Port_GameServer { get; set; } = 1234;

        /// <summary>
        /// The port of our chat server
        /// </summary>
        public static Int32 Port_ChatServer { get; set; } = 1235;


    }
}
