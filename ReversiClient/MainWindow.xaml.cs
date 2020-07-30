using ClientServerLibrary;
using Models.ReversiClient;
using Reversi.Models;
using ReversiClient.ViewModels;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ReversiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This is the ClientViewModel
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Properties
        /// <summary>
        /// The view model associated with this UI
        /// </summary>
        public static ReversiClientViewModel ThisClient { get; private set; }
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            ThisClient = new ReversiClientViewModel(new ReversiClientModel());
            this.DataContext = ThisClient;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hacky routine to allow the UI to update in the middle of a method.
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
            #region Connecting to Server

            // Check if the player name is valid
            if (String.IsNullOrEmpty(tbPlayerName.Text))
            {
                ThisClient.ConnectionStatusString = "You must enter a user name.";
                return;
            }

            // If the client is already connected to the game server, then don't
            // allow another connection.
            string status_string = String.Empty;
            if (ThisClient.IsConnectedToGameServer)
            {
                ThisClient.ConnectionStatusString = "You are already connected to the server!";
                return;
            }

            // Otherwise attempt to make the connection
            ThisClient.IsConnectedToGameServer = ThisClient.Model.MakeConnection(out status_string);
            if(!ThisClient.IsConnectedToGameServer)
            {
                ThisClient.ConnectionStatusString = status_string;
                return;
            }

            // If our socket is not connected, or we have lost link... 
            if (!ThisClient.Model.ServerSocket.Connected)
            {
                ThisClient.IsConnectedToGameServer = false;
                ThisClient.ConnectionStatusString = "Error connecting to socket.";
                return;
            }

            string name = tbPlayerName.Text;
            // Check if the name entered is a valid string
            if(String.IsNullOrEmpty(tbPlayerName.Text))
            {
                ThisClient.ConnectionStatusString = "Invalid name!";
                return;
            }

            // Send our login name to the server
            if (String.IsNullOrEmpty(name))
            {
                ThisClient.ConnectionStatusString = "A valid name must be entered.";
                ThisClient.IsConnectedToGameServer = false;
                return;
            }

            // Create our player object and send to the server
            Player newPlayer = new Player(-1, Players.UNDEFINED, name, ThisClient.Model.ServerSocket);
            Client.SerializeData<Player>(newPlayer, ThisClient.Model.ServerSocket);

            // Retrieve the accepted player data from the server
            ThisClient.ThisPlayer = DataTransmission.DeserializeData<Player>(ThisClient.Model.ServerSocket);

            if(ThisClient.ThisPlayer.IDType == Players.UNDEFINED)
            {
                // Update the UI
                spMakeConnection.Visibility = Visibility.Visible;
                spActiveGameRegion.Visibility = Visibility.Collapsed;

                ThisClient.ConnectionStatusString = "Connection refused by server.";

                // close the socket
                ThisClient.Model.ServerSocket.Close();  
                return;
            } else
            {
                // Now make the game area visible
                spMakeConnection.Visibility = Visibility.Collapsed;
                spActiveGameRegion.Visibility = Visibility.Visible;
            }
            #endregion

            #region Create a listening thread
            ThisClient.Model.ListenThread = new Thread(ThisClient.ListenServer);
            ThisClient.Model.ListenThread.Start();
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

            // Parse the results of the text box.
            if (Int32.TryParse(tbIndex.Text, out result))
            {
                if (result < 0)
                {
                    ThisClient.GameplayStatusString = "Invalid entry";
                    return;
                }
                else
                {
                    // Send move to server
                    ThisClient.Model.LastMove = new ReversiGameMove(ThisClient.ThisPlayer.PlayerID, result);
                    DataTransmission.SerializeData<ReversiGameMove>(ThisClient.Model.LastMove, ThisClient.Model.ServerSocket);

                    // Create the packet info.
                    ThisClient.GameplayStatusString = "Processing Move. Waiting for response from Server...";
                    
                }
            }
        }
        #endregion
    }
}
