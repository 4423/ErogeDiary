using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ErogeDiary.ViewModels.Pages;

public static class SupportedLanguages
{
    public static readonly LanguageOption Japanese = new("ja", "日本語", CultureInfo.GetCultureInfo("ja"));
    public static readonly LanguageOption English = new("en", "English", CultureInfo.GetCultureInfo("en"));

    public static IReadOnlyList<LanguageOption> All { get; } =
    [
        Japanese,
        English,
    ];

    public static LanguageOption FindOrDefault(string code)
        => All.FirstOrDefault(x => x.Code == code) ?? Japanese;

    public static CultureInfo GetCultureOrDefault(string code)
        => FindOrDefault(code).Culture;
}

public record LanguageOption(string Code, string DisplayName, CultureInfo Culture);
