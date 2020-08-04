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
        public static ReversiClientViewModel ThisClientVM { get; private set; }
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            ThisClientVM = new ReversiClientViewModel(new ReversiClientModel());
            this.DataContext = ThisClientVM;
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
                ThisClientVM.ConnectionStatusString = "You must enter a user name.";
                return;
            }

            // If the client is already connected to the game server, then don't
            // allow another connection.
            string status_string = String.Empty;
            if (ThisClientVM.IsConnectedToGameServer)
            {
                ThisClientVM.ConnectionStatusString = "You are already connected to the server!";
                return;
            }

            // Otherwise attempt to make the connection
            ThisClientVM.IsConnectedToGameServer = ThisClientVM.Model.MakeConnection(out status_string);
            if(!ThisClientVM.IsConnectedToGameServer)
            {
                ThisClientVM.ConnectionStatusString = status_string;
                return;
            }

            // If our socket is not connected, or we have lost link... 
            if (!ThisClientVM.Model.ServerSocket.Connected)
            {
                ThisClientVM.IsConnectedToGameServer = false;
                ThisClientVM.ConnectionStatusString = "Error connecting to socket.";
                return;
            }

            string name = tbPlayerName.Text;
            // Check if the name entered is a valid string
            if(String.IsNullOrEmpty(tbPlayerName.Text))
            {
                ThisClientVM.ConnectionStatusString = "Invalid name!";
                return;
            }

            // Send our login name to the server
            if (String.IsNullOrEmpty(name))
            {
                ThisClientVM.ConnectionStatusString = "A valid name must be entered.";
                ThisClientVM.IsConnectedToGameServer = false;
                return;
            }

            // Create our player object and send to the server
            PlayerModel newPlayer = new PlayerModel(-1, Players.UNDEFINED, name, ThisClientVM.Model.ServerSocket);
            ClientModel.SerializeData<PlayerModel>(newPlayer, ThisClientVM.Model.ServerSocket);

            // Retrieve the accepted player data from the server
            ThisClientVM.ThisPlayer = DataTransmission.DeserializeData<PlayerModel>(ThisClientVM.Model.ServerSocket);

            if(ThisClientVM.ThisPlayer.IDType == Players.UNDEFINED)
            {
                // Update the UI
                spMakeConnection.Visibility = Visibility.Visible;
                spActiveGameRegion.Visibility = Visibility.Collapsed;

                ThisClientVM.ConnectionStatusString = "Connection refused by server.";

                // close the socket
                ThisClientVM.Model.ServerSocket.Close();  
                return;
            } else
            {
                // Now make the game area visible
                spMakeConnection.Visibility = Visibility.Collapsed;
                spActiveGameRegion.Visibility = Visibility.Visible;
            }
            #endregion

            #region Create a listening thread
            ThisClientVM.Model.ListenThread = new Thread(ThisClientVM.ListenServer);
            ThisClientVM.Model.ListenThread.Start();
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
                    ThisClientVM.GameplayStatusString = "Invalid entry";
                    return;
                }
                else
                {
                    // Send move to server
                    ThisClientVM.Model.LastMove = new GameMoveModel(ThisClientVM.ThisPlayer.PlayerID, result);
                    DataTransmission.SerializeData<GameMoveModel>(ThisClientVM.Model.LastMove, ThisClientVM.Model.ServerSocket);

                    // Create the packet info.
                    ThisClientVM.GameplayStatusString = "Processing Move. Waiting for response from Server...";
                    
                }
            }
        }
        #endregion
    }
}
