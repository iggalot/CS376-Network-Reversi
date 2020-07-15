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
        ///  The port of our game server
        /// </summary>
        public static Int32 Port_GameServer { get; set; } = 1234;

        /// <summary>
        /// The port of our chat server
        /// </summary>
        public static Int32 Port_ChatServer { get; set; } = 1235;

        /// <summary>
        /// The maximum number of connections that the server can handle
        /// </summary>
        public const int MaxConnections = 8;

        /// <summary>
        /// The number of players in allowed in a game.
        /// </summary>
        public const int PlayersPerGame = 2;
    }
}
