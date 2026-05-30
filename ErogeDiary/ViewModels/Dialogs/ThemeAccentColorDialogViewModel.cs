using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErogeDiary.Helpers;
using Prism.Services.Dialogs;
using System.Windows.Media;

namespace ErogeDiary.ViewModels.Dialogs;

public partial class ThemeAccentColorDialogViewModel : BindableDialogBase
{
    public const string ThemeAccentColorParameterName = "themeAccentColor";

    [ObservableProperty]
    private Color themeAccentColor;

    [ObservableProperty]
    private string themeAccentColorCode = string.Empty;

    partial void OnThemeAccentColorChanged(Color value)
    {
        UpdateThemeAccentColorCode(value);
    }

    partial void OnThemeAccentColorCodeChanged(string value)
    {
        if (!ColorCodeHelper.TryParse(value, out var color))
        {
            return;
        }

        if (ThemeAccentColor != color)
        {
            ThemeAccentColor = color;
        }

        UpdateThemeAccentColorCode(color);
    }

    private void UpdateThemeAccentColorCode(Color value)
    {
        var colorCode = ColorCodeHelper.ToColorCode(value);
        if (ThemeAccentColorCode != colorCode)
        {
            ThemeAccentColorCode = colorCode;
        }
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        ThemeAccentColor = parameters.GetValue<Color>(ThemeAccentColorParameterName);
    }

    [RelayCommand]
    private void Apply()
    {
        var parameters = new DialogParameters
        {
            { ThemeAccentColorParameterName, ThemeAccentColor }
        };
        RaiseRequestClose(new DialogResult(ButtonResult.OK, parameters));
    }
}
