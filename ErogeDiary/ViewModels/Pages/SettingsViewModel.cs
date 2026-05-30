using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErogeDiary.Helpers;
using ErogeDiary.Models;
using ErogeDiary.Properties;
using ErogeDiary.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.ViewModels.Pages;

public partial class SettingsViewModel(IDialogService dialogService) : ObservableObject
{
    public IReadOnlyList<LanguageOption> LanguageOptions { get; } = SupportedLanguages.All;

    [ObservableProperty]
    private LanguageOption selectedLanguage = SupportedLanguages.FindOrDefault(Settings.Default.Language);

    partial void OnSelectedLanguageChanged(LanguageOption value)
    {
        Settings.Default.Language = value.Code;
        Settings.Default.Save();
    }

    [ObservableProperty]
    private Color themeAccentColor = Models.ThemeAccentColor.ParseOrDefault(Settings.Default.ThemeAccentColor);

    [RelayCommand]
    private void ChangeThemeAccentColor()
    {
        var parameters = new DialogParameters
        {
            { ThemeAccentColorDialogViewModel.ThemeAccentColorParameterName, ThemeAccentColor }
        };

        dialogService.ShowDialog(nameof(Views.Dialogs.ThemeAccentColorDialog), parameters, result =>
        {
            if (result.Result != ButtonResult.OK)
            {
                return;
            }

            var color = result.Parameters.GetValue<Color>(ThemeAccentColorDialogViewModel.ThemeAccentColorParameterName);
            ThemeAccentColor = color;
            Settings.Default.ThemeAccentColor = ColorCodeHelper.ToColorCode(color);
            Settings.Default.Save();
        });
    }
}
