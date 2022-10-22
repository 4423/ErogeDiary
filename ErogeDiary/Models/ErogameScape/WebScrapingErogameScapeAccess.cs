using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ErogeDiary.Models.ErogameScape
{
    public class WebScrapingErogameScapeAccess : IErogameScapeAccess
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<GameInfo> GetGameInfoFromGamePageUrl(string url)
        {
            var id = Regex.Match(url, "game=(\\d+)*").Groups[1].Value;

            IHtmlDocument html;
            using (var stream = await httpClient.GetStreamAsync(url))
            {
                var parser = new HtmlParser();
                html = await parser.ParseDocumentAsync(stream);
            }

            var title = html.QuerySelector("#game_title > a").TextContent;
            var brand = html.QuerySelector("#brand > td > a").TextContent;
            var releaseDate = html.QuerySelector("#sellday > td > a").TextContent;
            var imageUrl = ((IHtmlImageElement)html.QuerySelector("#main_image img")).Source;
            
            return new GameInfo()
            {
                Id = id,
                Title = title,
                Brand = brand,
                ReleaseDate = DateTime.Parse(releaseDate),
                ImageUri = imageUrl
            };
        }
    }
}
