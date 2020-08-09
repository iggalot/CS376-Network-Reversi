using System;

namespace ClientServerLibrary
{
    [Serializable]
    public enum ServerTypes
    {
        ServerUnknown = -1,
        ServerServermanager = 0,
        ServerGameserver = 1,
        ServerChatserver = 2
    }

    [Serializable]
    public enum ConnectionStatusTypes
    {
        StatusConnectionIncapable = -2,
        StatusConnectionUnknown = -1,
        StatusConnectionAccepted = 0,
        StatusConnectionRefused = 1,
        StatusConnectionError = 2
        
    }
}
