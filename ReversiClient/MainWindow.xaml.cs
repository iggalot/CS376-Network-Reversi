using ClientServerLibrary;
using Reversi.Models;
using ReversiClient.ViewModels;
using Settings;
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

            // Reset the status string
            ThisClientVM.ConnectionStatusString = String.Empty;

            // Check if the player name is valid
            string name = tbPlayerName.Text;
            if (String.IsNullOrEmpty(name))
            {
                ThisClientVM.ConnectionStatusString = "A valid name must be entered.";
                return;
            }

            if (String.IsNullOrEmpty(name))
            {
                ThisClientVM.ConnectionStatusString = "You must enter a user name.";
                return;
            }

            // If the client is already connected to the game server, then don't
            // allow another connection.
            string status_string = String.Empty;
            if (ThisClientVM.IsConnectedToServer)
            {
                ThisClientVM.ConnectionStatusString = "You are already connected to the server!";
                return;
            }

            // Otherwise attempt to make the connection
            ThisClientVM.IsConnectedToServer = ThisClientVM.Model.ConnectClient(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer, out status_string);
            if(!ThisClientVM.IsConnectedToServer)
            {
                ThisClientVM.ConnectionStatusString = status_string;
                return;
            }

            // If our socket is not connected, or we have lost link... 
            if (DataTransmission.SocketConnected(ThisClientVM.Model.ConnectionSocket.Client) == false)
            {
                ThisClientVM.ConnectionStatusString = "Error connecting to socket.";
                return;
            }

            // Create our player object and send to the server
            ReversiClientModel model = new ReversiClientModel(ThisClientVM.Model.ConnectionSocket, null);
            model.CurrentStatus = ConnectionStatusTypes.STATUS_CONNECTION_ACCEPTED;

            //PlayerModel newPlayer = new PlayerModel(-1, Players.UNDEFINED, name, ThisClientVM.Model.ServerSocket);
            ReversiClientModel.SerializeData<ReversiClientModel>(model, ThisClientVM.Model.ConnectionSocket);

            // Retrieve the data from the server
            model = DataTransmission.DeserializeData<ReversiClientModel>(ThisClientVM.Model.ConnectionSocket);
            model.ConnectionSocket = ThisClientVM.Model.ConnectionSocket; // Must readd the Connection socket as a parameter on this Client Model since it isnt serialized

            // Create a ReversiClientModel from the received client model
            // This sets the PlayerID that was assigned to the client model
            ReversiClientModel temp = new ReversiClientModel(model, name);

            // TODO:  Create the player object from the client model object
            ThisClientVM.ThisPlayer = temp.ClientPlayer;

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
                    DataTransmission.SerializeData<GameMoveModel>(ThisClientVM.Model.LastMove, ThisClientVM.Model.ConnectionSocket);

                    // Create the packet info.
                    ThisClientVM.GameplayStatusString = "Processing Move. Waiting for response from Server...";
                    
                }
            }
        }
        #endregion
    }
}
