using Reversi.ViewModels;
using ReversiClient.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ReversiClient.ValueConverters
{
    public class BoardIndexToCanvasPositionValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)values[0];
            int dim = (int)values[1];

            if (String.Equals((string)parameter, "LEFT"))
            {
                return ReversiGameboardVM.CanvasLeftPosition(index,dim);
            }
            else
            {
                return ReversiGameboardVM.CanvasTopPosition(index,dim);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
