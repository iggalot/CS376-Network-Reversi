using ClientServerLibrary;
using Models.ReversiClient;
using Reversi.Models;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace ReversiClient.ViewModels
{
    public class ReversiClientViewModel : BaseViewModel
    {
        #region Private Properties
        private string _gameplayStatusString = string.Empty;
        private string _connectionStatusString = string.Empty;
        private string _packetStatusString = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// The client model associated with this view model
        /// </summary>
        public ReversiClientModel Model { get; private set; }

        /// <summary>
        /// The current game being controlled by this client
        /// </summary>
        public ReversiGame CurrentGame
        {
            get => Model.Game;
            set
            {
                if (value == null)
                    return;

                SetGame(value);
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
        public Player ThisPlayer { 
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
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set the game associated with this client
        /// </summary>
        /// <param name="game"></param>
        private void SetGame(ReversiGame game)
        {
            Model.Game = game;
        }

        /// <summary>
        /// Set the player associated with this client
        /// </summary>
        /// <param name="player"></param>
        private void SetPlayer(Player player)
        {
            Model.ClientPlayer = player;
        }

        /// <summary>
        /// The thread callback function that listens for data from the server and updates
        /// the various model objects
        /// </summary>
        public void ListenServer()
        {
            NetworkStream stream = Model.ServerSocket.GetStream();
            Application.Current.Dispatcher.Invoke(() =>
            {
                PacketStatusString = "Client:  Listen thread started.";
            }
            );

            while (!Model.ClientShouldShutdown)
            {
                if (stream.DataAvailable)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PacketStatusString = "Client:  Data on thread detected.";
                    });
                    try
                    {
                        CurrentGame = DataTransmission.DeserializeData<ReversiGame>(Model.ServerSocket);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PacketStatusString = "Game data received";
                        });
                    }
                    catch
                    {
                        try
                        {
                            Model.LastMove = DataTransmission.DeserializeData<ReversiGameMove>(Model.ServerSocket);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PacketStatusString = "GameMove data received";
                            });
                        }
                        catch
                        {
                            ThisPlayer = DataTransmission.DeserializeData<Player>(Model.ServerSocket);
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
