using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.Models;

public class AccentColors: List<AccentColor>
{
    public AccentColors()
    {
        // https://valleysoftware.com.au/development/windows-accent-colours/
        Add("#FFB900"); //Yellow Gold
        Add("#FF8C00"); //Gold
        Add("#F7630C"); //Orange Bright
        Add("#CA5010"); //Orange Dark
        Add("#DA3B01"); //Rust
        Add("#EF6950"); //Pale Rust
        Add("#D13438"); //Brick Red
        Add("#FF4343"); //Mod Red

        Add("#E74856"); //Pale Red
        Add("#E81123"); //Red
        Add("#EA005E"); //Rose Bright
        Add("#C30052"); //Rose
        Add("#E3008C"); //Plum Light
        Add("#BF0077"); //Plum
        Add("#C239B3"); //Orchid Light
        Add("#9A0089"); //Orchid

        Add("#0078D7"); //Default Blue
        Add("#0063B1"); //Navy Blue
        Add("#8E8CD8"); //Purple Shadow
        Add("#6B69D6"); //Purple Shadow Dark
        Add("#8764B8"); //Iris Pastel
        Add("#744DA9"); //Iris Spring
        Add("#B146C2"); //Violet Red Light
        Add("#881798"); //Violet Red

        Add("#0099BC"); //Cool Blue Bright
        Add("#2D7D9A"); //Cool Blue
        Add("#00B7C3"); //Seafoam
        Add("#038387"); //Seafoam Team
        Add("#00B294"); //Mint Light
        Add("#018574"); //Mint Dark
        Add("#00CC6A"); //Turf Green
        Add("#10893E"); //Sport Green
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
