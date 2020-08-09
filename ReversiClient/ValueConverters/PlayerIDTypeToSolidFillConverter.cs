using Reversi.Models;
using System;
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

            switch (type)
            {
                case Players.Undefined:
                    return Brushes.Transparent;
                case Players.Player1:
                    return Brushes.White;
                case Players.Player2:
                    return Brushes.Black;
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
