using ErogeDiary.Models;
using ErogeDiary.ViewModels.Pages;
using System.Linq;
using Xunit;

namespace ErogeDiary.Tests.ViewModels.Pages;

public class SettingsViewModelTests
{
    [Fact]
    public void Constructor_SelectsSavedLanguage()
    {
        var settings = new StubApplicationSettingsStore { Language = "en" };

        var viewModel = new SettingsViewModel(settings);

        Assert.Equal("en", viewModel.SelectedLanguage.Code);
    }

    [Fact]
    public void SelectedLanguageChanged_SavesLanguage()
    {
        var settings = new StubApplicationSettingsStore { Language = "ja" };
        var viewModel = new SettingsViewModel(settings);

        viewModel.SelectedLanguage = viewModel.LanguageOptions.Single(x => x.Code == "en");

        Assert.Equal("en", settings.Language);
        Assert.Equal(1, settings.SaveCount);
    }

    [Fact]
    public void Constructor_FallsBackToJapaneseForUnknownLanguage()
    {
        var settings = new StubApplicationSettingsStore { Language = "unknown" };

        var viewModel = new SettingsViewModel(settings);

        Assert.Equal("ja", viewModel.SelectedLanguage.Code);
    }

    private sealed class StubApplicationSettingsStore : IApplicationSettingsStore
    {
        public string Language { get; set; } = "ja";

        public bool UpgradeRequired { get; set; }

        public int SaveCount { get; private set; }

        public void Save()
        {
            SaveCount++;
        }

        public void Upgrade()
        {
        }
    }
}
