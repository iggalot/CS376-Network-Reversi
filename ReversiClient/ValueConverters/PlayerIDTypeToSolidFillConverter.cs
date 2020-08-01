using Reversi.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReversiClient.ValueConverters
{
    public class PlayerIDTypeToSolidFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Players type = (Players)value;
            SolidColorBrush brush;

            switch (type)
            {
                case Players.UNDEFINED:
                    brush = Brushes.Transparent;
                    break;
                case Players.PLAYER1:
                    brush = Brushes.White;
                    break;
                case Players.PLAYER2:
                    brush = Brushes.Black;
                    break;
                default:
                    brush = Brushes.Transparent;
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
