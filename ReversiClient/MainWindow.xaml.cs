using ClientServerLibrary;
using Reversi.Models;
using ReversiClient.ViewModels;
using Settings;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GameObjects.Models;
using Reversi.ViewModels;

namespace ReversiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This is the ClientViewModel
    /// </summary>
    public partial class MainWindow
    {
        private static ReversiClientViewModel _thisClientVm;

        #region Public Properties

        /// <summary>
        /// The view model associated with this UI
        /// </summary>
        public static ReversiClientViewModel ThisClientVm
        {
            get => _thisClientVm;
            private set
            {
                if ((value == null) || (value == _thisClientVm))
                    return;

                _thisClientVm = value;
            }
        }

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            ThisClientVm = new ReversiClientViewModel(new ReversiClientModel());
            this.DataContext = ThisClientVm;


        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hacky routine to allow the UI to update in the middle of a method.
        /// </summary>
        void AllowUiToUpdate()
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
            ThisClientVm.RCVMTestString = "Button connect clicked";
            #region Connecting to Server
            
            // Reset the status string
            ThisClientVm.ConnectionStatusString = String.Empty;

            // Check if the player name is valid
            string name = tbPlayerName.Text;
            if (String.IsNullOrEmpty(name))
            {
                ThisClientVm.ConnectionStatusString = "A valid name must be entered.";
                return;
            }

            if (String.IsNullOrEmpty(name))
            {
                ThisClientVm.ConnectionStatusString = "You must enter a user name.";
                return;
            }

            // If the client is already connected to the game server, then don't
            // allow another connection.
            if (ThisClientVm.IsConnectedToServer)
            {
                ThisClientVm.ConnectionStatusString = "You are already connected to the server!";
                return;
            }

            // Otherwise attempt to make the connection
            ThisClientVm.IsConnectedToServer = ThisClientVm.Model.ConnectClient(GlobalSettings.ServerAddress, GlobalSettings.Port_GameServer, out var statusString);
            if(!ThisClientVm.IsConnectedToServer)
            {
                ThisClientVm.ConnectionStatusString = statusString;
                return;
            }

            // If our socket is not connected, or we have lost link... 
            if (DataTransmission.SocketConnected(ThisClientVm.Model.ConnectionSocket.Client) == false)
            {
                ThisClientVm.ConnectionStatusString = "Error connecting to socket.";
                return;
            }

            ThisClientVm.IsConnectedToServer = true;

            // Create our player object and send to the server.
            // Id is currently -1 since its not assigned yet.
            ReversiClientModel model = new ReversiClientModel(ThisClientVm.Model.ConnectionSocket, null)
            {
                ClientPlayer = new PlayerModel(-1, Players.Undefined, name)
            };

            // Send the client model to the server
            ReversiClientModel.SerializeData<ReversiClientModel>(model, ThisClientVm.Model.ConnectionSocket);

            // Retrieve the updated client model data from the server
            model = DataTransmission.DeserializeData<ReversiClientModel>(ThisClientVm.Model.ConnectionSocket);
            ThisClientVm.UpdateNonSerializeableObjects(ThisClientVm, model);

            // Create a ReversiClientModel from the received client model
            // This sets the PlayerID that was assigned to the client model
            ReversiClientModel temp = new ReversiClientModel(model, name);

            ThisClientVm.Model = model;
            ThisClientVm.ThisPlayerViewModel = new ReversiPlayerVM(temp);
            #endregion

            ThisClientVm.IsWaitingForGameStart = true;

            #region Create a listening thread to listen for updates
            ThisClientVm.Model.ListenThread = new Thread(ThisClientVm.ListenServerThreadCallback);
            ThisClientVm.Model.ListenThread.Start();
            #endregion
        }

        /// <summary>
        /// The action for when the move submission button is clicked.  This will be changed when we get to UI clicking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_SubmitMoveClick(object sender, RoutedEventArgs e)
        {
            ThisClientVm.RCVMTestString = "submit clicked";

            GameMoveSubmittedEventArgs args = new GameMoveSubmittedEventArgs();
            args.MoveIndex = Int32.Parse(tbIndex.Text);
            args.TimeReceived = DateTime.Now;
            OnGameMoveSubmitted(args);

            //Parse the results of the text box.
            if (Int32.TryParse(tbIndex.Text, out var result))
            {
                if (result < 0)
                {
                    ThisClientVm.GameplayStatusString = "Invalid entry";
                    return;
                }
                else
                {
                    // Send move to server
                    ThisClientVm.Model.LastMove = new GameMoveModel(ThisClientVm.ThisPlayerViewModel.IdType, result);
                    DataTransmission.SerializeData<GameMoveModel>(ThisClientVm.Model.LastMove, ThisClientVm.Model.ConnectionSocket);
                }
            }
        }
        #endregion

        #region Events and Handlers

        public event EventHandler MoveSubmitted;

        protected virtual void OnGameMoveSubmitted(GameMoveSubmittedEventArgs e)
        {
            EventHandler handler = MoveSubmitted;
            handler?.Invoke(this, e);

            ThisClientVm.PacketStatusString = "Move submitted for index " + e.MoveIndex + " at " + e.TimeReceived.ToString();
        }



        public class GameMoveSubmittedEventArgs : EventArgs
        {
            public int MoveIndex { get; set; }
            public DateTime TimeReceived { get; set; }
        }


        #endregion

    }
}
