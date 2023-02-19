using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.Models;

public class AccentColors: List<AccentColor>
{
    public AccentColors()
    {
        Add("#d46262"); //Light Red
        Add("#d48062"); //Light Orange
        Add("#d4a862"); //Light Yellow
        Add("#b5bf71"); //Light Uguisu
        Add("#88b56c"); //Light YellowGreen
        Add("#63a678"); //Light Green
        Add("#63a69e"); //Light Asagi
        Add("#6389a6"); //Light Skyblue
        Add("#636da6"); //Light Blue
        Add("#8463a6"); //Light Purple
        Add("#a6639b"); //Light Pink
        Add("#a66378"); //Light Azuki
        Add("#ba2d2d"); //Dark Red
        Add("#c75734"); //Dark Orange
        Add("#cf9436"); //Dark Yellow
        Add("#a2b038"); //Dark Uguisu
        Add("#62a638"); //Dark YellowGreen
        Add("#339652"); //Dark Green
        Add("#33968a"); //Dark Asagi
        Add("#336b96"); //Dark Skyblue
        Add("#334296"); //Dark Blue
        Add("#643396"); //Dark Purple
        Add("#963386"); //Dark Pink
        Add("#963352"); //Dark Azuki
        Add("#949494"); //Gray 1
        Add("#808080"); //Gray 2
        Add("#696969"); //Gray 3
        Add("#545454"); //Gray 4
    }

    private void Add(string colorCode)
    {
        Add(new AccentColor((Color)ColorConverter.ConvertFromString(colorCode)));
    }
}

public record AccentColor(Color Color)
{
    public SolidColorBrush Brush => new SolidColorBrush(Color);
}
