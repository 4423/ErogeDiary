using System;
using System.Threading;
using System.Windows.Media;
using Xunit;

namespace ErogeDiary.Controls.ModernColorPicker.Tests;

public class ModernColorPickerTests
{
    [Fact]
    public void SelectedColorChanged_UpdatesColorCode()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker
            {
                SelectedColor = Color.FromRgb(0x12, 0xAB, 0xF0)
            };

            Assert.Equal("#12ABF0", picker.ColorCode);
        });
    }

    [Fact]
    public void ColorCodeChanged_UpdatesSelectedColor()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker
            {
                ColorCode = "#336B96"
            };

            Assert.Equal(Color.FromRgb(0x33, 0x6B, 0x96), picker.SelectedColor);
        });
    }

    [Fact]
    public void InvalidColorCode_KeepsPreviousColorCodeAndSelectedColor()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker
            {
                ColorCode = "#336B96"
            };

            picker.ColorCode = "336B96";

            Assert.Equal("#336B96", picker.ColorCode);
            Assert.Equal(Color.FromRgb(0x33, 0x6B, 0x96), picker.SelectedColor);
        });
    }

    [Fact]
    public void SelectedColorChanged_ForcesOpaqueRgbColor()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker
            {
                SelectedColor = Color.FromArgb(0x40, 0x12, 0x34, 0x56)
            };

            Assert.Equal(Color.FromRgb(0x12, 0x34, 0x56), picker.SelectedColor);
            Assert.Equal("#123456", picker.ColorCode);
        });
    }

    [Fact]
    public void ColorCodeChanged_UpdatesFieldBaseColor()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker
            {
                ColorCode = "#336B96"
            };
            var expected = GetFieldBaseColor(ColorPickerState.FromColor(Color.FromRgb(0x33, 0x6B, 0x96), 0));

            Assert.Equal(expected, picker.FieldBaseColor);
        });
    }

    [Fact]
    public void SelectedColorChanged_WithAlpha_UpdatesFieldBaseColor()
    {
        RunOnStaThread(() =>
        {
            var picker = new ModernColorPicker();
            var initialFieldBaseColor = picker.FieldBaseColor;

            picker.SelectedColor = Color.FromArgb(0x40, 0x12, 0x34, 0x56);
            var expected = GetFieldBaseColor(ColorPickerState.FromColor(Color.FromRgb(0x12, 0x34, 0x56), 0));

            Assert.NotEqual(initialFieldBaseColor, picker.FieldBaseColor);
            Assert.Equal(expected, picker.FieldBaseColor);
        });
    }

    private static Color GetFieldBaseColor(ColorPickerState state) =>
        ColorPickerColorConverter.Mix(
            ColorPickerColorConverter.ColorFromHsv(state.Hue, 1, 1),
            Colors.White,
            state.Tint);

    private static void RunOnStaThread(Action test)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                test();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            throw exception;
        }
    }
}
