using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErogeDiary.Helpers;
using ErogeDiary.Models;
using ErogeDiary.Properties;
using ErogeDiary.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IApplicationSettingsStore settingsStore;
        private readonly IDialogService dialogService;

        public SettingsViewModel(IApplicationSettingsStore settingsStore, IDialogService dialogService)
        {
            this.settingsStore = settingsStore;
            this.dialogService = dialogService;
            selectedLanguage = SupportedLanguages.FindOrDefault(settingsStore.Language);
            themeAccentColor = Models.ThemeAccentColor.ParseOrDefault(Settings.Default.ThemeAccentColor);
        }

        public IReadOnlyList<LanguageOption> LanguageOptions { get; } = SupportedLanguages.All;

        [ObservableProperty]
        private LanguageOption selectedLanguage;

        partial void OnSelectedLanguageChanged(LanguageOption value)
        {
            settingsStore.Language = value.Code;
            settingsStore.Save();
        }

        [ObservableProperty]
        private Color themeAccentColor;

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
}
