using System.Windows.Media;
using Xunit;

namespace ErogeDiary.Controls.ModernColorPicker.Tests;

public class ColorPickerStateTests
{
    [Fact]
    public void ToColor_CombinesHueTintAndBrightness()
    {
        var state = new ColorPickerState(240, 0.5, 0.5);

        Assert.Equal(Color.FromRgb(64, 64, 128), state.ToColor());
    }

    [Fact]
    public void FromColor_RoundTripsColor()
    {
        var color = Color.FromRgb(0x76, 0x51, 0xD4);
        var state = ColorPickerState.FromColor(color, 0);

        Assert.Equal(color, state.ToColor());
    }

    [Fact]
    public void FromColor_UsesFallbackHueForBlack()
    {
        var state = ColorPickerState.FromColor(Colors.Black, 123);

        Assert.Equal(123, state.Hue);
        Assert.Equal(0, state.Brightness);
    }

    [Fact]
    public void FromColor_UsesFallbackHueForWhite()
    {
        var state = ColorPickerState.FromColor(Colors.White, 123);

        Assert.Equal(123, state.Hue);
        Assert.Equal(1, state.Tint);
        Assert.Equal(1, state.Brightness);
    }
}
