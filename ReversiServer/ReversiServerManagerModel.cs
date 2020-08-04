using ClientServerLibrary;
using Settings;

namespace ReversiServer
{
    public class ReversiServerManagerModel : ServerManagerModel
    {
        public ReversiServerManagerModel() : base(ServerTypes.SERVER_SERVERMANAGER,GlobalSettings.ServerAddress,GlobalSettings.Port_GameServer)
        {

        }

        /// <summary>
        /// The main thread handle for this reversi server.
        /// </summary>
        public override void HandleServerThread()
        {
            // Check for enough clients to play the game
        }
    }
}
