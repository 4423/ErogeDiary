using ErogeDiary.Properties;
using ModernWpf;
using System.Collections.Generic;
using System.Linq;

namespace ErogeDiary.ViewModels.Pages;

public static class SupportedThemes
{
    public static readonly ThemeOption Dark = new("Dark", Strings.Settings_ThemeDark, ApplicationTheme.Dark);
    public static readonly ThemeOption Light = new("Light", Strings.Settings_ThemeLight, ApplicationTheme.Light);

    public static IReadOnlyList<ThemeOption> All { get; } =
    [
        Dark,
        Light,
    ];

    public static ThemeOption FindOrDefault(string code)
        => All.FirstOrDefault(x => x.Code == code) ?? Dark;
}

public record ThemeOption(string Code, string DisplayName, ApplicationTheme Theme);
