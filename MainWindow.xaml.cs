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
            ReversiGame game = new ReversiGame(2);
            CurrentGame = game.Instance;
        }

        private void Button_SubmitMoveClick(object sender, RoutedEventArgs e)
        {
            int result;
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

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(packet.FormPacket());
            //            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Test name from client" + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            IsWaitingForResponse = true;  // waiting for a response from the server.

            // Await the server response
            string response = Client.Receive(serverStream);

            // Now make the game area visible
            spMakeConnection.Visibility = Visibility.Collapsed;
            spActiveGameRegion.Visibility = Visibility.Visible;

            // If the client is null or empty, wait for a period then try again
            while (String.IsNullOrEmpty(response))
            {
                Thread.Sleep(1000);
                response = Client.Receive(serverStream);
            }

            // Unpack the contents of the packet that were received from the server.
            PacketInfo receivepacket = new PacketInfo();
            receivepacket.UnpackPacket(response);

            // Display results in the window
            lbPlayerID.Content = receivepacket.Id;
            lbCurrentPlayer.Content = receivepacket.Data;
            lbStatus.Content = receivepacket.Type;
        }
    }
}
