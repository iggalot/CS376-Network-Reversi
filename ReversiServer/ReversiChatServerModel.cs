using ClientServerLibrary;
using Reversi.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ReversiServer
{
    public class ReversiChatServerModel : ServerModel
    {
        #region Private Properties

        private Dictionary<int, ReversiChatModel> runningChatModelsList = new Dictionary<int, ReversiChatModel>();

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="address">The address to the server</param>
        /// <param name="port">The port for the server</param>
        public ReversiChatServerModel(string address, Int32 port) : base(ServerTypes.ServerChatserver, address, port)
        {
            Console.WriteLine("---- Creating a new Reversi Chat Server with ID:" + Id.ToString());

            this.StartServer();
        }

        #endregion

        #region Virtual Override Methods

        public override void StartServer()
        {
            base.StartServer();

            // TODO:  Start threads
            // TODO:  Create listener
            // TODO:  Create game model

        }

        public override void Update()
        {
            base.Update();
        }
        #endregion

        #region Thread Callbacks

        /// <summary>
        /// The main thread for the chat server
        /// </summary>
        private static void ChatServerThread()
        {
            Console.WriteLine("... ChatServer: Chat server started");
            while (true)
            {
                Thread.Sleep(3000);
                Console.WriteLine("...ChatServer: chat server is idle");
            }
        }
        #endregion
    }
}
