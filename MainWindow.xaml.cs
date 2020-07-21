using ClientServerLibrary;
using Settings;
using Reversi.Models;
using System;
using System.Net.Sockets;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace Reversi
{
    public class ReversiGame
    {
        /// <summary>
        /// The instance for our game
        /// </summary>
        public Game Instance { get; set; }
        public ReversiGame(int num_players)
        {
            // Start the game with specified numer of players
            Instance = new Game(num_players);

            // Setup the gameboard
            Instance.SetupGame();

            // Start the game
            Instance.PlayGame();
        }

        public ReversiGame(Player p1, Player p2)
        {
            // Start the game with specified numer of players
            Instance = new Game(p1, p2);

            // Setup the gameboard
            Instance.SetupGame();

            // Start the game
            Instance.PlayGame();
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The current game being controlled by this client
        /// </summary>
        public Game CurrentGame { get; set; }

        /// <summary>
        /// The current player associated with this client
        /// </summary>
        public Player PlayerInfo { get; set; }
        

        /// <summary>
        /// The connection to the server has been made.
        /// </summary>
        public bool IsConnectedToGameServer { get; set; } = false;

        /// <summary>
        /// Is waiting for a response from the server.
        /// </summary>
        public bool IsWaitingForResponse { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();

            // TODO:  Remove this instance creation here...for testing purposes only.  Must revise the ButtonClick routine below.
            //ReversiGame game = new ReversiGame(2);
            //CurrentGame = game.Instance;

            // Create player placeholder
            PlayerInfo = new Player(Players.UNDEFINED, "unknown", null);
        }

        private void Button_SubmitMoveClick(object sender, RoutedEventArgs e)
        {
            int result;

            //// Send move to to server
            //PacketInfo packet = new PacketInfo(1, "40", PacketType.PACKET_GAMEMOVE_REQUEST);
            //DataTransmission.SendData(client, packet);


            // Wait for response

            // Process display of results



            //// Parse the results
            if (Int32.TryParse(tbIndex.Text, out result))
            {
                if(result < 0)
                {
                    lbStatus.Content = "Invalid entry";
                    return;
                } else
                {
                    CurrentGame.CurrentMoveIndex = result;
                    lbIndex.Content = CurrentGame.CurrentMoveIndex;
                    lbCurrentPlayer.Content = (CurrentGame.CurrentPlayer.ID + CurrentGame.CurrentPlayer.Name);

                    lbStatus.Content = "Processing Move";
                    
                    CurrentGame.PlaySounds(GameSounds.SOUND_CLICK_SUCCESSFUL);

                    CurrentGame.PlayRound();
                    tbGameboard.Text = CurrentGame.Gameboard.DrawGameboard();

                    lbStatus.Content = "Move Successful";

                    CurrentGame.PlaySounds(GameSounds.SOUND_TURN_COMPLETE);
                }
            }
        }

        /// <summary>
        /// The button that makes the connection to the game server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ConnectClick(object sender, RoutedEventArgs e)
        {
            TcpClient clientSocket;
            NetworkStream serverStream;
            PacketInfo receivePacket = new PacketInfo();  // a palceholder packet

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
            if(String.IsNullOrEmpty(tbPlayerName.Text))
            {
                lbConnectStatus.Content = "You must enter a user name.";
                return;
            }

            // Otherwise try to make the connection
            try
            {           
                clientSocket = Client.Connect(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
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
            if (!clientSocket.Connected)
            {
                IsConnectedToGameServer = false;
                lbConnectStatus.Visibility = Visibility.Visible;
                lbConnectStatus.Content = "Error connecting to socket.";
                return;
            }

            string name = tbPlayerName.Text;
            // Send our login name to the server
            if(String.IsNullOrEmpty(name))
            {
                IsConnectedToGameServer = false;
                lbConnectStatus.Visibility = Visibility.Visible;
                lbConnectStatus.Content = "A valid name must be entered.";
                return;
            }

            // Create the packet info.  Send ID of -1 to signal that we need a server id to be assigned to this player
            PacketInfo packet = new PacketInfo(-1, name, PacketType.PACKET_CONNECTION_REQUEST);

            DataTransmission.SendData(clientSocket, packet);

            // waiting for a response from the server.
            IsWaitingForResponse = true;

            // Await the server response
            Console.WriteLine("Client:  waiting for response from server...");
            receivePacket = new PacketInfo();
            Client.ReceiveData(clientSocket, out receivePacket);


            IsWaitingForResponse = false;

            switch (receivePacket.Type)
            {
                case PacketType.PACKET_UNDEFINED:
                    break;
                case PacketType.PACKET_CONNECTION_REQUEST:
                        break;
                case PacketType.PACKET_CONNECTION_ACCEPTED:
                    {
                        // Now make the game area visible
                        spMakeConnection.Visibility = Visibility.Collapsed;
                        spActiveGameRegion.Visibility = Visibility.Visible;

                        // Once verified, create our player object
                        PlayerInfo.ID = Players.PLAYER1;
                        PlayerInfo.Name = receivePacket.Data;
                        PlayerInfo.Socket = clientSocket;

                        // Display results in the window
                        lbPlayerID.Content = receivePacket.Id;
                        lbCurrentPlayer.Content = receivePacket.Data;
                        lbStatus.Content = receivePacket.Type;
                        lbPacketStatus.Content = receivePacket.Type;
                        break;
                    }
                case PacketType.PACKET_CONNECTION_REFUSED:
                    {
                        // Now make the game area visible
                        spMakeConnection.Visibility = Visibility.Visible;
                        spActiveGameRegion.Visibility = Visibility.Collapsed;
                        lbConnectStatus.Content = receivePacket.Data;
                        lbPacketStatus.Content = receivePacket.Type;

                        IsConnectedToGameServer = false;
                        
                        break;
                    }
                case PacketType.PACKET_GAME_STARTING:
                    break;
                case PacketType.PACKET_GAMEMOVE_REQUEST:
                    break;
                case PacketType.PACKET_GAMEMOVE_ACCEPTED:
                    break;
                case PacketType.PACKET_GAMEMOVE_DENIED:
                    break;
                case PacketType.PACKET_GAME_ENDING:
                    break;
                default:
                    break;
            }

            // Wait for the server to signal that the game has begun.
            receivePacket = new PacketInfo();
            Client.ReceiveData(clientSocket, out receivePacket);

            lbConnectStatus.Content = receivePacket.Data;
            lbStatus.Content = receivePacket.Data;
            lbPacketStatus.Content = receivePacket.Type;


            // Wait for the server to signal that the game has begun.
            receivePacket = new PacketInfo();
            Client.ReceiveData(clientSocket, out receivePacket);

            lbStatus.Content = receivePacket.ToString(); ;

            lbConnectStatus.Content = receivePacket.Data;
            lbStatus.Content = receivePacket.Data;
            lbPacketStatus.Content = receivePacket.Type;

            // Send the gameboard packet string to be unpacked
            tbGameboard.Text = Board.UnpackGameboardPacketString(receivePacket.Data);
        }
    }
}
