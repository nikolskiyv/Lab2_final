using System.Numerics;
using System.Windows.Data;
using System;
using ClassLibrary;

namespace WpfApp1
{
    [ValueConversion(typeof(Grid2D), typeof(string))]
    public class GridConverter : IValueConverter
    {
        public object Convert(object Value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Grid2D Grid = (Grid2D)Value;
            string Str = $"Grid X: Step = {Grid.StepX}, Step Num = {Grid.NumOfNodesX}\n" +
                         $"Grid Y: Step = {Grid.StepY}, Step Num = {Grid.NumOfNodesY}\n "; ;
            return Str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}