using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace ErogeDiary.ErogameScape;

public class ErogameScapeClient
{
    private static readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// </summary>
    /// <param name="gamePageUrl">
    /// <example>https://erogamescape.dyndns.org/~ap2/ero/toukei_kaiseki/game.php?game=13050</example>
    /// </param>
    /// <returns></returns>
    public async Task<GameInfo> FetchGameInfoAsync(string gamePageUrl)
    {
        var id = Regex.Match(gamePageUrl, "game=(\\d+)*").Groups[1].Value;

        IHtmlDocument html;
        using (var stream = await httpClient.GetStreamAsync(gamePageUrl))
        {
            var parser = new HtmlParser();
            html = await parser.ParseDocumentAsync(stream);
        }

        var title = html.QuerySelector("#game_title > a").TextContent;
        var brand = html.QuerySelector("#brand > td > a").TextContent;
        var releaseDate = html.QuerySelector("#sellday > td > a").TextContent;
        var imageUrl = ((IHtmlImageElement)html.QuerySelector("#main_image img")).Source;
        
        return new GameInfo(
            Id: id,
            Title: title,
            Brand: brand,
            ReleaseDate: DateOnly.Parse(releaseDate),
            ImageUri: imageUrl
        );
    }
}
