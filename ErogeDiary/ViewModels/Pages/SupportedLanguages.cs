using System.Collections.Generic;
using System.Linq;

namespace ErogeDiary.ViewModels.Pages;

public static class SupportedLanguages
{
    public static readonly LanguageOption Japanese = new("ja", "日本語");
    public static readonly LanguageOption English = new("en", "English");

    public static IReadOnlyList<LanguageOption> All { get; } =
    [
        Japanese,
        English,
    ];

    public static LanguageOption FindOrDefault(string code)
        => All.FirstOrDefault(x => x.Code == code) ?? Japanese;
}

public record LanguageOption(string Code, string DisplayName);
