namespace ClientServerLibrary
{
    public enum ServerTypes
    {
        SERVER_UNKNOWN = -1,
        SERVER_SERVERMANAGER = 0,
        SERVER_GAMESERVER = 1,
        SERVER_CHATSERVER = 2
    }

    public enum ConnectionStatusTypes
    {
        STATUS_CONNECTION_INCAPABLE = -2,
        STATUS_CONNECTION_UNKNOWN = -1,
        STATUS_CONNECTION_ACCEPTED = 0,
        STATUS_CONNECTION_REFUSED = 1,
        STATUS_CONNECTION_ERROR = 2
        
    }
}
