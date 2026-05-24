using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ErogeDiary.Controls.ModernColorPicker;

public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Color color
            ? new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B))
            : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SolidColorBrush brush
            ? Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B)
            : Binding.DoNothing;
    }
}
