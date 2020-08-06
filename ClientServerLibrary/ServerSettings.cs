using System;

namespace ClientServerLibrary
{
    public static class ServerSettings
    {
        /// <summary>
        /// The maximum number of connections that the server can handle
        /// </summary>
        public const int MaxServerConnections = 1;

        /// <summary>
        /// The counter before a timeout is declared
        /// </summary>
        public const int MaxServerTimeoutCount = 150;

        /// <summary>
        /// The default delay between update cycles on the update thread
        /// </summary>
        public const int ServerUpdatePulseDelay = 2000;
    }
}
