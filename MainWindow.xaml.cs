using ClientServerLibrary;
using Settings;
using Reversi.Models;
using System;
using System.Net.Sockets;
using System.Windows;

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
        public Game CurrentGame { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            // TODO:  Remove this instance creation here...for testing purposes only.  Must revise the ButtonClick routine below.
            ReversiGame game = new ReversiGame(2);
            CurrentGame = game.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

                    //using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows User Account Control.wav"))
                    //{
                    //    soundPlayer.Play();
                    //}

                    CurrentGame.PlayRound();
                    tbGameboard.Text = CurrentGame.Gameboard.DrawGameboard();

                    lbStatus.Content = "Move Successful";

                    CurrentGame.PlaySounds(GameSounds.SOUND_TURN_COMPLETE);

                    //using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\tada.wav"))
                    //{
                    //    soundPlayer.Play();
                    //}
                }

            }
        }

        private void Button_ConnectClick(object sender, RoutedEventArgs e)
        {
            TcpClient clientSocket;
            NetworkStream serverStream;

            string address = GlobalSettings.ServerAddress;

            

            // Otherwise try to make the connection
            try
            {
                
                clientSocket = Client.Connect(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer);
                serverStream = clientSocket.GetStream();
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

            //readData = "Connected to Chat Server...";
            //msg();

            // If our socket is not connected, 
            if (!clientSocket.Connected)
            {
                //lbConnectStatus.Visibility = Visibility.Visible;
                //lbConnectStatus.Content = "Error connected to socket.";
                //isConnected = false;
                return;
            }

            // Send our login name to the server
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Test name from client" + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

        }
    }
}
