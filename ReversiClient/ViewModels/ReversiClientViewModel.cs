using System;
using System.Collections.Generic;
using ClientServerLibrary;
using Reversi.Models;
using Reversi.ViewModels;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameObjects.Models;

namespace ReversiClient.ViewModels
{
    public class ReversiClientViewModel : BaseViewModel
    {
        private string _rcvmteststring;


        public string RCVMTestString
        {
            get => _rcvmteststring;
            set
            {
                _rcvmteststring = value ?? throw new ArgumentNullException(nameof(value));

                OnPropertyChanged("RCVMTestString");
            }
        } 


        #region Private Properties
        private string _gameplayStatusString = string.Empty;
        private string _connectionStatusString = string.Empty;
        private string _packetStatusString = string.Empty;
        private bool _isConnectedToServer = false;
        private bool _isWaitingForGameStart = false;

        // new
        private ReversiGameVM _reversiReversiGameVm = null;
        private ReversiPlayerVM _thisReversiPlayerVm;
        private ReversiClientModel _model;

        #endregion

        #region Public Properties

        /// <summary>
        /// The client model associated with this view model
        /// </summary>
        public ReversiClientModel Model
        {
            get => _model;
            set
            {
                if((value == null) || (value == _model))
                {
                    return;
                }

                _model = value;

                OnPropertyChanged("Model");
            }
        }

        /// <summary>
        /// The game view model controlled by this client window
        /// </summary>
        public ReversiGameVM ReversiGameViewModel { 
            get => _reversiReversiGameVm;
            set
            {
                if (value?.Model == null)
                    return;

                _reversiReversiGameVm = value;

                OnPropertyChanged("ReversiGameViewModel");
                OnPropertyChanged("GameboardVM");
            }
        }

        public ReversiPlayerVM ThisPlayerViewModel
        {
            get => _thisReversiPlayerVm;
            set
            {
                if (value == null)
                    return;

                _thisReversiPlayerVm = value;

                OnPropertyChanged("ThisPlayerViewModel");
            }
        }




        ///// <summary>
        ///// The player object associated with this UI
        ///// </summary>
        //public PlayerModel ThisPlayer { 
        //    get => Model.ClientPlayer;
        //    set
        //    {
        //        if (value == null)
        //            return;

        //        SetPlayer(value);
        //        OnPropertyChanged("ThisPlayer");
        //    } 
        //}


        /// <summary>
        /// Holds the gameplay status string for this client
        /// </summary>
        public string GameplayStatusString
        {
            get =>_gameplayStatusString;
            set
            {
                if (value == null)
                    return;

                _gameplayStatusString = value;
                OnPropertyChanged("GameplayStatusString");
            }
        }

        /// <summary>
        /// Holds the packet status message
        /// </summary>
        public string PacketStatusString
        {
            get => _packetStatusString;
            set
            {
                if (value == null)
                    return;

                _packetStatusString = value;
                OnPropertyChanged("PacketStatusString");
            }
        }

        /// <summary>
        /// Holds the connection status message
        /// </summary>
        public string ConnectionStatusString
        {
            get => _connectionStatusString;
            set
            {
                if (value == null)
                    return;

                _connectionStatusString = value;
                OnPropertyChanged("ConnectionStatusString");
            }
        }

        /// <summary>
        /// Is waiting for a response from the server.
        /// </summary>
        public bool IsWaitingForGameStart
        {
            get => _isWaitingForGameStart;
            set
            {
                if (value == _isWaitingForGameStart)
                    return;

                _isWaitingForGameStart = value;
                OnPropertyChanged("IsWaitingForGameStart");
            }
        }

        /// <summary>
        /// Is waiting for a response from the server.
        /// </summary>
        public bool IsConnectedToServer 
        {
            get => _isConnectedToServer;
            set
            {
                if (value == _isConnectedToServer)
                    return;

                _isConnectedToServer = value;

                OnPropertyChanged("IsConnectedToServer");
            } 
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="model"></param>
        public ReversiClientViewModel(ReversiClientModel model)
        {
            Model = model;

            if (model == null)
                return;

            // TODO: DO we construct off of the CurrentGame constructor?:
            if (model.Game != null)
            {
                SetGame(model.Game);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Copies the nonserialized data objects including sockets and threads from the original view model
        /// To a new mode.  Needed after a Deserialize() request.
        /// </summary>
        /// <param name="vm">the client view model to update</param>
        /// <param name="model">the new client model</param>
        public void UpdateNonSerializeableObjects(ReversiClientViewModel vm, ReversiClientModel model)
        {
            // Must re-add the Connection socket and other nonserialized objects since
            // these parameters on this Client Model are not serialized and are lost after transmission
            model.ConnectionSocket = vm.Model.ConnectionSocket;
            model.ListenerSocket = vm.Model.ListenerSocket;
            model.ListenThread = vm.Model.ListenThread;
            model.ClientProcess = vm.Model.ClientProcess;
            model.ClientMainThread = vm.Model.ClientMainThread;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set the game associated with this client
        /// </summary>
        /// <param name="game">The game object to be set</param>
        private void SetGame(ReversiGameModel game)
        {
            // check for a valid game object
            if (game == null)
                return;
            else
            {
                if (ThisPlayerViewModel.IdType == Players.Undefined)
                {
                    foreach (KeyValuePair<int,ClientModel> item in game.CurrentPlayersList)
                    {
                        ReversiClientModel reversiPlayer = (ReversiClientModel) item.Value;
                        if (ThisPlayerViewModel.PlayerId == reversiPlayer.ClientPlayer.PlayerId)
                        {
                            ThisPlayerViewModel.IdType = reversiPlayer.ClientPlayer.IdType;
                            break;
                        }

                    }
                }
                // assign the game model and update the game view model
                Model.Game = game;
                ReversiGameViewModel = new ReversiGameVM(game);
            }
        }

        /// <summary>
        /// Set the player associated with this client
        /// </summary>
        /// <param name="player"></param>
        private void SetPlayer(PlayerModel player)
        {
            Model.ClientPlayer = player;
        }

        /// <summary>
        /// Display the gameboard string
        /// </summary>
        public string GameboardDisplayString
        {
            get
            {
                if (Model.Game == null)
                    return string.Empty;
                else if (Model.Game.Gameboard == null)
                    return string.Empty;
                else
                    return Model.Game.Gameboard.DrawGameboardString();
            }
        }

        /// <summary>
        /// The thread callback function that listens for data from the server and updates
        /// the various model objects.
        /// </summary>
        public void ListenServerThreadCallback()
        {
            NetworkStream stream = Model.ConnectionSocket.GetStream();
            Application.Current.Dispatcher.Invoke(() =>
            {
                PacketStatusString = "Client:  Listen thread started.";
            }
            );

            // The main listening loop
            while (!Model.ShouldShutdown)
            {
                // Check if the main thread of the client application is still alive
                if (!Model.ClientMainThread.IsAlive)
                {
                    Model.ShouldShutdown = true;
                    continue;
                }

                // Check if the parent process is still running
                if (Model.ClientProcess.HasExited)
                {
                    Model.ShouldShutdown = true;
                    continue;
                }

                // Check if the Socket is still connected.  If not, exit and gracefully shutdown the thread.
                if (!DataTransmission.SocketConnected(Model.ConnectionSocket.Client))
                {
                    Model.ShouldShutdown = true;
                    continue;
                }

                // Check for any incoming data on the stream.
                if (stream.DataAvailable)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PacketStatusString = "Client:  Data on thread detected.";
                    });
                    try
                    {
                        ReversiGameModel currentGame = DataTransmission.DeserializeData<ReversiGameModel>(Model.ConnectionSocket);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PacketStatusString = "Game data received";
                        });

                        GameUpdateReceivedEventArgs args = new GameUpdateReceivedEventArgs() {TimeReceived = DateTime.Now};
                        OnGameUpdateReceived(args);

                        // Set the game model to trigger OnProperty events.
                        SetGame(currentGame);
                        
                    }
                    catch
                    {
                        try
                        {
                            Model.LastMove = DataTransmission.DeserializeData<GameMoveModel>(Model.ConnectionSocket);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PacketStatusString = "GameMove data received";
                            });
                        }
                        catch
                        {
                            //ReversiClientModel = DataTransmission.DeserializeData<ReversiClientModel>(Model.ConnectionSocket);
                            //Application.Current.Dispatcher.Invoke(() =>
                            //{
                            //    PacketStatusString = "Player data received";
                            //});
                        }

                    }
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    PacketStatusString = "Client:  Data on thread detected.";
                    //});
                }

            }
        }


        #endregion

        #region Events
        public event EventHandler GameUpdateReceived;

        protected virtual void OnGameUpdateReceived(GameUpdateReceivedEventArgs e)
        {
            EventHandler handler = GameUpdateReceived;
            handler?.Invoke(this, e);

            PacketStatusString = "Game update received from server at " + e.TimeReceived.ToString();
        }

        public class GameUpdateReceivedEventArgs : EventArgs
        {
            public DateTime TimeReceived { get; set; }
        }
        #endregion



    }




}
