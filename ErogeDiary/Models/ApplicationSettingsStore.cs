using ErogeDiary.Properties;

namespace ErogeDiary.Models;

public interface IApplicationSettingsStore
{
    string Language { get; set; }

    bool UpgradeRequired { get; set; }

    void Save();

    void Upgrade();
}

public class ApplicationSettingsStore : IApplicationSettingsStore
{
    public string Language
    {
        get => Settings.Default.Language;
        set => Settings.Default.Language = value;
    }

    public bool UpgradeRequired
    {
        get => Settings.Default.UpgradeRequired;
        set => Settings.Default.UpgradeRequired = value;
    }

    public void Save()
    {
        Settings.Default.Save();
    }

    public void Upgrade()
    {
        Settings.Default.Upgrade();
    }
}
