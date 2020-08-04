using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
using System.Collections.Generic;

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
        /// <param name="server_type">The type of server <see cref="ServerTypes"/></param>
        /// <param name="address">The address to the server</param>
        /// <param name="port">The port for the server</param>
        public ReversiChatServerModel(string address, Int32 port) : base(ServerTypes.SERVER_CHATSERVER, address, port)
        {
            Console.WriteLine("---- Creating a new Reversi Chat Server with ID:" + ID.ToString());

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
    }
}
