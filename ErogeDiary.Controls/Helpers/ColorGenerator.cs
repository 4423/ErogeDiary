using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ErogeDiary.Controls.Helpers;

internal static class ColorGenerator
{
    private static List<SolidColorBrush> colors = new List<SolidColorBrush>();
    private static List<SolidColorBrush> pallet;

    static ColorGenerator()
    {
        var colorCodes = new string[]
        {
            "#665191",
            "#a05195",
            "#d45087",
            "#f95d6a",
            "#ff7c43",
            "#ffa600",
            "#003f5c",
            "#2f4b7c",
        };
        pallet = colorCodes.Select(colorCode => 
            (SolidColorBrush)new BrushConverter().ConvertFromString(colorCode)!
        ).ToList();
    }

    public static IEnumerable<SolidColorBrush> Generate(int count)
    {
        if (colors.Count < count)
        {
            colors.AddRange(pallet);
        }

        return colors.Take(count);
    }
}
