using ClientServerLibrary;
using Models.ReversiClient;
using Reversi.Models;
using Reversi.ViewModels;
using System.Net.Sockets;
using System.Windows;

namespace ReversiClient.ViewModels
{
    public class ReversiClientViewModel : BaseViewModel
    {
        #region Private Properties
        private string _gameplayStatusString = string.Empty;
        private string _connectionStatusString = string.Empty;
        private string _packetStatusString = string.Empty;

        // new
        private ReversiGameVM _reversiGameVM = null;

        #endregion

        #region Public Properties
        /// <summary>
        /// The client model associated with this view model
        /// </summary>
        public ReversiClientModel Model { get; private set; }

        /// <summary>
        /// The game view model controlled by this client window
        /// </summary>
        public ReversiGameVM GameViewModel { 
            get => _reversiGameVM;
            set
            {
                if (value == null)
                    return;

                if (value.Model == null)
                    return;

                _reversiGameVM = value;

                OnPropertyChanged("GameViewModel");
                OnPropertyChanged("GameboardVM");
            }
        }

        /// <summary>
        /// The current game being controlled by this client
        /// </summary>
        public ReversiGameModel CurrentGame
        {
            get => GetGame();
            set
            {
                if (value == null)
                    return;

                // Assigns this game object to the underlying model's game object
                SetGame(value);

                // Update the corresponding game view model
                GameViewModel = new ReversiGameVM(value);

                OnPropertyChanged("CurrentGame");
                OnPropertyChanged("GameboardDisplayString");
            }
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
        /// The player object associated with this UI
        /// </summary>
        public PlayerModel ThisPlayer { 
            get => Model.ClientPlayer;
            set
            {
                if (value == null)
                    return;

                SetPlayer(value);
                OnPropertyChanged("ThisPlayer");
            } 
        }

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
        public bool IsWaitingForResponse { get; set; } = false;

        /// <summary>
        /// Is waiting for a response from the server.
        /// </summary>
        public bool IsConnectedToGameServer { get; set; } = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="model"></param>
        public ReversiClientViewModel(ReversiClientModel model)
        {
            Model = model;

            // TODO: DO we construct off of the CurrentGame constructor?:
            if (model.Game != null)
            {
                GameViewModel = new ReversiGameVM(model.Game);
                //CurrentGame = model.Game;                
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set the game associated with this client
        /// </summary>
        /// <param name="game"></param>
        private void SetGame(ReversiGameModel game)
        {
            Model.Game = game;
        }

        /// <summary>
        /// Retrtieve the underlying game object
        /// </summary>
        /// <returns></returns>
        private ReversiGameModel GetGame()
        {
            return Model.Game;
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
        /// The thread callback function that listens for data from the server and updates
        /// the various model objects.
        /// </summary>
        public void ListenServer()
        {
            NetworkStream stream = Model.ServerSocket.GetStream();
            Application.Current.Dispatcher.Invoke(() =>
            {
                PacketStatusString = "Client:  Listen thread started.";
            }
            );

            // The main listening loop
            while (!Model.ClientShouldShutdown)
            {
                // Check if the main thread of the client application is still alive
                if (!Model.MainThread.IsAlive)
                {
                    Model.ClientShouldShutdown = true;
                    continue;
                }

                // Check if the parent process is still running
                if (Model.ReversiClientProcess.HasExited)
                {
                    Model.ClientShouldShutdown = true;
                    continue;
                }

                // Check if the Socket is still connected.  If not, exit and gracefully shutdown the thread.
                if (!DataTransmission.SocketConnected(Model.ServerSocket.Client))
                {
                    Model.ClientShouldShutdown = true;
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
                        CurrentGame = DataTransmission.DeserializeData<ReversiGameModel>(Model.ServerSocket);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PacketStatusString = "Game data received";
                        });
                    }
                    catch
                    {
                        try
                        {
                            Model.LastMove = DataTransmission.DeserializeData<GameMoveModel>(Model.ServerSocket);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PacketStatusString = "GameMove data received";
                            });
                        }
                        catch
                        {
                            ThisPlayer = DataTransmission.DeserializeData<PlayerModel>(Model.ServerSocket);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PacketStatusString = "Player data received";
                            });
                        }

                    }
                }
            }
        }

        #endregion


    }


}
