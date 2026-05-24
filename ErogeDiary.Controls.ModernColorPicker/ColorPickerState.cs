using System.Windows.Media;

namespace ErogeDiary.Controls.ModernColorPicker;

internal readonly record struct ColorPickerState(double Hue, double Tint, double Brightness)
{
    public Color ToColor()
    {
        var hueColor = ColorPickerColorConverter.ColorFromHsv(Hue, 1, 1);
        var tintedColor = ColorPickerColorConverter.Mix(hueColor, Colors.White, Tint);
        return Color.FromRgb(
            (byte)Math.Round(tintedColor.R * Brightness),
            (byte)Math.Round(tintedColor.G * Brightness),
            (byte)Math.Round(tintedColor.B * Brightness));
    }

    public static ColorPickerState FromColor(Color color, double fallbackHue)
    {
        var opaqueColor = Color.FromRgb(color.R, color.G, color.B);
        var max = Math.Max(opaqueColor.R, Math.Max(opaqueColor.G, opaqueColor.B));
        var brightness = max / 255d;

        if (brightness <= 0)
        {
            return new ColorPickerState(fallbackHue, 0, 0);
        }

        var unshadedColor = Color.FromRgb(
            (byte)Math.Min(255, Math.Round(opaqueColor.R / brightness)),
            (byte)Math.Min(255, Math.Round(opaqueColor.G / brightness)),
            (byte)Math.Min(255, Math.Round(opaqueColor.B / brightness)));

        var min = Math.Min(unshadedColor.R, Math.Min(unshadedColor.G, unshadedColor.B));
        var tint = min / 255d;

        if (tint >= 1)
        {
            return new ColorPickerState(fallbackHue, tint, brightness);
        }

        var hueColor = Color.FromRgb(
            ColorPickerColorConverter.RemoveTint(unshadedColor.R, tint),
            ColorPickerColorConverter.RemoveTint(unshadedColor.G, tint),
            ColorPickerColorConverter.RemoveTint(unshadedColor.B, tint));
        return new ColorPickerState(ColorPickerColorConverter.GetHue(hueColor), tint, brightness);
    }
}
