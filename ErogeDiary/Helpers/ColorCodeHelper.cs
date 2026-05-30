using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace ErogeDiary.Helpers;

public static class ColorCodeHelper
{
    private static readonly Regex HexRgbPattern = new("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    public static bool TryParse(string? colorCode, out Color color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(colorCode) || !HexRgbPattern.IsMatch(colorCode))
        {
            return false;
        }

        color = Color.FromRgb(
            r: byte.Parse(colorCode.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            g: byte.Parse(colorCode.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            b: byte.Parse(colorCode.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
        );
        return true;
    }

    public static string ToColorCode(Color color) =>
        FormattableString.Invariant($"#{color.R:X2}{color.G:X2}{color.B:X2}");
}
