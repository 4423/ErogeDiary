using System.Windows.Media;

namespace ErogeDiary.Controls.ModernColorPicker;

internal static class ColorPickerColorConverter
{
    public static Color Mix(Color color, Color otherColor, double amount)
    {
        var inverse = 1 - amount;
        return Color.FromRgb(
            (byte)Math.Round(color.R * inverse + otherColor.R * amount),
            (byte)Math.Round(color.G * inverse + otherColor.G * amount),
            (byte)Math.Round(color.B * inverse + otherColor.B * amount));
    }

    public static byte RemoveTint(byte channel, double tint)
    {
        var value = (channel - 255 * tint) / (1 - tint);
        return (byte)Math.Round(Clamp(value / 255) * 255);
    }

    public static double GetHue(Color color)
    {
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        if (delta == 0)
        {
            return 0;
        }

        double hue;
        if (max == r)
        {
            hue = 60 * (((g - b) / delta) % 6);
        }
        else if (max == g)
        {
            hue = 60 * ((b - r) / delta + 2);
        }
        else
        {
            hue = 60 * ((r - g) / delta + 4);
        }

        return hue < 0 ? hue + 360 : hue;
    }

    public static Color ColorFromHsv(double hue, double saturation, double value)
    {
        var chroma = value * saturation;
        var x = chroma * (1 - Math.Abs(hue / 60 % 2 - 1));
        var m = value - chroma;

        var (r, g, b) = hue switch
        {
            < 60 => (chroma, x, 0d),
            < 120 => (x, chroma, 0d),
            < 180 => (0d, chroma, x),
            < 240 => (0d, x, chroma),
            < 300 => (x, 0d, chroma),
            _ => (chroma, 0d, x),
        };

        return Color.FromRgb(
            (byte)Math.Round((r + m) * 255),
            (byte)Math.Round((g + m) * 255),
            (byte)Math.Round((b + m) * 255));
    }

    public static double Clamp(double value) =>
        Math.Max(0, Math.Min(1, value));
}
