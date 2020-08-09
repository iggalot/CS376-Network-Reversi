using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReversiClient.ValueConverters
{
    /// <summary>
    /// Returns a visibility.visible when value = true and if parameter is not specified.
    /// Returns a visibility.collapsed object when value = true and if parameter is "INVERT"
    /// </summary>
    public class BoolToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasPiece = (bool)value;

            if(String.Equals((string)parameter, "INVERT"))
            {
                return (hasPiece == true) ? Visibility.Collapsed : Visibility.Visible;
            } else
            {
                return (hasPiece == true) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
