using ErogeDiary.Helpers;
using System.Windows.Media;

namespace ErogeDiary.Models;

public static class ThemeAccentColor
{
    public static Color DefaultColor = Color.FromRgb(0x5B, 0x2C, 0x6F);

    public static Color ParseOrDefault(string? colorCode)
    {
        if (ColorCodeHelper.TryParse(colorCode, out var color))
        {
            return color;
        }

        return DefaultColor;
    }
}
