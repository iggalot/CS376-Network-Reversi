using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Reversi.ViewModels;

namespace ReversiClient.Views
{
    /// <summary>
    /// Interaction logic for GameboardView.xaml
    /// </summary>
    public partial class ReversiGameboardView : UserControl
    {
        public ReversiGameboardView()
        {
            InitializeComponent();
        }

        private void GameSquare_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var rect = sender as Rectangle;
            var dc = rect.DataContext as ReversiSquareVM;

            if (dc.Model.HasPiece)
                return;

            rect.Fill = Brushes.LightGreen;
        }

        private void GameSquare_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var rect = sender as Rectangle;
            var dc = rect.DataContext as ReversiSquareVM;

            if (dc.Model.HasPiece)
                return;

            rect.Fill = Brushes.DarkGreen;
        }

        /// <summary>
        /// The event to fire when the square is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameSquare_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as FrameworkElement;
            var myObject = obj.DataContext as ReversiSquareVM;

            if (myObject.Model.HasPiece)
                return;

            MessageBox.Show("Index: " + myObject.Model.Index + " was clicked");
        }
    }
}
