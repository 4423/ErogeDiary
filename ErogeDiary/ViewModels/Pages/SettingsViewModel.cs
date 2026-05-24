using CommunityToolkit.Mvvm.ComponentModel;
using ErogeDiary.Models;
using System.Collections.Generic;

namespace ErogeDiary.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IApplicationSettingsStore settingsStore;

        public SettingsViewModel(IApplicationSettingsStore settingsStore)
        {
            this.settingsStore = settingsStore;
            selectedLanguage = SupportedLanguages.FindOrDefault(settingsStore.Language);
        }

        public IReadOnlyList<LanguageOption> LanguageOptions { get; } = SupportedLanguages.All;

        [ObservableProperty]
        private LanguageOption selectedLanguage;

        partial void OnSelectedLanguageChanged(LanguageOption value)
        {
            settingsStore.Language = value.Code;
            settingsStore.Save();
        }
    }
}
