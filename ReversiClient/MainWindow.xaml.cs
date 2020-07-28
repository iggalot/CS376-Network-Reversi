using ClientServerLibrary;
using Reversi.Models;
using Settings;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ReversiClient
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Properties
        private int _playerID;
        private TcpClient _serverSocket;

        #endregion

        #region Public Properties
        /// <summary>
        /// The current game being controlled by this client
        /// </summary>
        public ReversiGame CurrentGame { get; set; }

        ///// <summary>
        ///// The assigned id of the player.
        ///// </summary>
        //public int PlayerID { get; private set; }

        ///// <summary>
        ///// The current player associated with this client
        ///// </summary>
        //public Player PlayerInfo { get; set; }

        /// <summary>
        /// The connection to the server has been made.
        /// </summary>
        public bool IsConnectedToGameServer { get; set; } = false;

        /// <summary>
        /// Is waiting for a response from the server.
        /// </summary>
        public bool IsWaitingForResponse { get; set; } = false;

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// HAcky routine to allow the UI to update in the middle of a method.
        /// </summary>
        void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }
        #endregion

        #region UI Controls

        /// <summary>
        /// The button that makes the connection to the game server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ConnectClick(object sender, RoutedEventArgs e)
        {
            PacketInfo receivePacket = new PacketInfo();  // a palceholder packet

            #region Connecting to Server

            TcpClient clientSocket;
            NetworkStream serverStream;

            // retrieve the server address
            string address = GlobalSettings.ServerAddress;

            // If the client is already connected to the game server, then don't
            // allow another connection.
            if (IsConnectedToGameServer)
            {
                lbConnectStatus.Content = "You are already connected to the server!";
                return;
            }

            // Check if the player name is valid
            if (String.IsNullOrEmpty(tbPlayerName.Text))
            {
                lbConnectStatus.Content = "You must enter a user name.";
                return;
            }

            // Otherwise try to make the connection
            try
            {
                clientSocket = Client.Connect(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
                _serverSocket = clientSocket;  // save the socket once the connection is made
                serverStream = clientSocket.GetStream();
                IsConnectedToGameServer = true;

            }
            catch (ArgumentNullException excep)
            {
                Console.WriteLine("ArgumentNullException: {0}", excep);
                return;
            }
            catch (SocketException excep)
            {
                Console.WriteLine("SocketException: {0}", excep);
                return;
            }

            // If the connection is not successful...
            if (!IsConnectedToGameServer)
                return;

            //readData = "Connected to Chat Server...";
            //msg();

            // If our socket is not connected, or we have lost link... 
            if (!_serverSocket.Connected)
            {
                IsConnectedToGameServer = false;
                lbConnectStatus.Visibility = Visibility.Visible;
                lbConnectStatus.Content = "Error connecting to socket.";
                return;
            }

            string name = tbPlayerName.Text;

            if(String.IsNullOrEmpty(tbPlayerName.Text))
            {
                lbConnectStatus.Visibility = Visibility.Visible;
                lbConnectStatus.Content = "Invalid name!";
                return;
            }

            // Send our login name to the server
            if (String.IsNullOrEmpty(name))
            {
                IsConnectedToGameServer = false;
                lbConnectStatus.Visibility = Visibility.Visible;
                lbConnectStatus.Content = "A valid name must be entered.";
                return;
            }

            // Create our player object and send to the server
            Player newPlayer = new Player(-1, Players.UNDEFINED, name, _serverSocket);
            Client.SerializeData<Player>(newPlayer, _serverSocket);

            // Retrieve the accepted player data from the server
            newPlayer = DataTransmission.DeserializeData<Player>(_serverSocket);

            if(newPlayer.IDType == Players.UNDEFINED)
            {
                // Update the UI
                spMakeConnection.Visibility = Visibility.Visible;
                spActiveGameRegion.Visibility = Visibility.Collapsed;

                lbConnectStatus.Content = "Connection refused by server.";

                // close the socket
                _serverSocket.Close();  
                return;
            } else
            {
                _playerID = newPlayer.PlayerID; // remember the server assigned id number

                // Now make the game area visible
                spMakeConnection.Visibility = Visibility.Collapsed;
                spActiveGameRegion.Visibility = Visibility.Visible;

                // Display results in the window
                lbPlayer1ID.Content = newPlayer.PlayerID;
                lbPlayer1Name.Content = newPlayer.Name;
            }

            // Now receive the gameboard from the server
            if(_serverSocket.Connected)
            {
                // Receive the gameboard from the server
                CurrentGame = DataTransmission.DeserializeData<ReversiGame>(_serverSocket);
                UpdateUI();
            }
            #endregion

        }

        /// <summary>
        /// The action for when the move submission button is clicked.  This will be changed when we get to UI clicking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_SubmitMoveClick(object sender, RoutedEventArgs e)
        {
            int result;
            TcpClient clientSocket = _serverSocket;  // retrieve our socket
            PacketInfo receivePacket = new PacketInfo(); ;

            // Parse the results of the text box.
            if (Int32.TryParse(tbIndex.Text, out result))
            {
                if (result < 0)
                {
                    lbStatus.Content = "Invalid entry";
                    return;
                }
                else
                {
                    CurrentGame.CurrentMoveIndex = result;

                    // TODO: Remove this update. Display data on the screen (temporary)
                    lbIndex.Content = CurrentGame.CurrentMoveIndex;
                    lbCurrentPlayer.Content = ("Current Players turn: " + CurrentGame.CurrentPlayer);

                    //// Send move to to server
                    // Create the packet info.
                    lbStatus.Content = "Processing Move. Waiting for response from Server...";
                    PacketInfo packet = new PacketInfo(-1, CurrentGame.CurrentMoveIndex.ToString(), PacketType.PACKET_GAMEMOVE_REQUEST);
                    DataTransmission.SendData(clientSocket, packet);

                    // Wait for the server to signal whether the move was accepted or not.
                    receivePacket = new PacketInfo();
                    System.Threading.Thread.Sleep(1000);

                    while (!DataTransmission.ReceiveData(clientSocket, out receivePacket))
                    {
                        if (receivePacket == null)
                        {
                            lbStatus.Content = "Packet was null";
                            lbPacketStatus.Content = "null packet";
                            Console.WriteLine("Client: gameMovePacket was null for opponent");
                            return;
                        }
                        else if (receivePacket.Type == PacketType.PACKET_UNDEFINED)
                        {
                            lbStatus.Content = "Packet was undefined";
                            lbPacketStatus.Content = receivePacket.Type;
                            Console.WriteLine("Client: gameMovePacket type was UNDEFINED");
                            return;
                        }
                        // send a denied response if a move was received
                        else if (receivePacket.Type == PacketType.PACKET_GAMEMOVE_DENIED)
                        {
                            Console.WriteLine("Client: Move denied by server");
                            lbStatus.Content = "The move is invalid.";
                            lbPacketStatus.Content = receivePacket.Type;

                            AllowUIToUpdate();
                            // TODO:  Add reject sound

                            break;
                        }
                        else if (receivePacket.Type == PacketType.PACKET_GAMEMOVE_ACCEPTED) 
                        {
                            Console.WriteLine("Client: Move accepted by server");
                            lbStatus.Content = "The move is valid.  Updating board.";
                            lbPacketStatus.Content = receivePacket.Type;

                            //TODO:  Update board and apply move

                            ReversiSounds.PlaySounds(GameSounds.SOUND_CLICK_SUCCESSFUL);

                            CurrentGame.PlayRound();
                            tbGameboard.Text = CurrentGame.Gameboard.DrawGameboard();

                            AllowUIToUpdate();

                            ReversiSounds.PlaySounds(GameSounds.SOUND_TURN_COMPLETE);
                            break;
                        } 
                        else
                        {
                            lbStatus.Content = "Invalid or unknown packet type received";
                            lbPacketStatus.Content = receivePacket.Type;
                            Console.WriteLine("Client: Invalid packet of type " + receivePacket.Type + " was received.");
                            return;
                        }

                        //TODO:  Add a timeout delay here...
                    }
                }
            }
            lbStatus.Content = receivePacket.Data;
            lbPacketStatus.Content = receivePacket.Type;
        }
        #endregion

        /// <summary>
        /// Updates the UI once the gameboard object has been received.
        /// </summary>
        private void UpdateUI()
        {
            // Display results in the window
            lbGameID.Content = "GameID: (Id# " + CurrentGame.GameID + ")";

            Player current = CurrentGame.GetPlayerById(CurrentGame.CurrentPlayer);
            lbCurrentPlayer.Content = "Current Player: (Id# " + current.PlayerID + ") " + current.Name;

            lbPlayer1ID.Content = CurrentGame.CurrentPlayers[0].PlayerID;
            lbPlayer1Name.Content = CurrentGame.CurrentPlayers[0].Name;

            lbPlayer2ID.Content = CurrentGame.CurrentPlayers[1].PlayerID;
            lbPlayer2Name.Content = CurrentGame.CurrentPlayers[1].Name;

            tbGameboard.Text = CurrentGame.Gameboard.DrawGameboard();
        }

    }




}
