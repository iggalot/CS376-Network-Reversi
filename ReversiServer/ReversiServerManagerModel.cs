using ClientServerLibrary;
using Settings;

namespace ReversiServer
{
    public class ReversiServerManagerModel : ServerManagerModel
    {
        public ReversiServerManagerModel() : base(ServerTypes.SERVER_SERVERMANAGER,GlobalSettings.ServerAddress,GlobalSettings.Port_GameServer)
        {

        }


        //static void Main()
        //{
        //    // Setup our statemachine for this game
        //    //           ReversiStateMachine gs = new ReversiStateMachine();
        //    //           ReversiStateMachine.TestStateMachine();

        //    // Start the game server thread
        //    ThreadStart gameStart = new ThreadStart(GameServerThread);
        //    Thread gameThread = new Thread(gameStart);
        //    Console.WriteLine("Starting game server...");
        //    gameThread.Start();
        //    GameThreads.Add(gameThread);

        //    // Uncomment these lines to create the chat server thread
        //    //// Start the chat server thread
        //    //ThreadStart chatStart = new ThreadStart(ChatServerThread);
        //    //Thread chatThread = new Thread(chatStart);
        //    //Console.WriteLine("Starting chat server...");
        //    //chatThread.Start();
        //    //ChatThreads.Add(chatThread);

        //    gameThread.Join();

        //    // Uncomment this line to end the game when the chat thread completes.            
        //    //chatThread.Join();
        //}

    }
}
