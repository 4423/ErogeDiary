using System.Windows.Media;

namespace ErogeDiary.Helpers;

public static class SolidColorBrushHelpers
{
    public static SolidColorBrush fromColorCode(string colorCode)
    {
        var converter = new BrushConverter();
        return (SolidColorBrush)converter.ConvertFromString(colorCode)!;
    }
}
