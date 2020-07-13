using Reversi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reversi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game CurrentGame { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            // Start the game with specified numer of players
            CurrentGame = new Game(2);

            // Setup the gameboard
            CurrentGame.SetupGame();

            // Start the game
            CurrentGame.PlayGame();
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
    }
}
