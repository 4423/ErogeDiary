using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public static class ThumbnailDownloadHelper
    {
        private static readonly HttpClient client;

        static ThumbnailDownloadHelper()
        {
            client = new HttpClient();
            ThumbnailDir = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "thumbnails");
        }

        public static string ThumbnailDir { get; private set; }

        public async static Task<string> DownloadAsync(string imageUri)
        {
            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var extension = Path.GetExtension(imageUri);
            var outputPath = Path.Combine(ThumbnailDir, $"{timestamp}{extension}");

            await DownloadCoreAsync(imageUri, outputPath);
            
            return outputPath;
        }

        private async static Task DownloadCoreAsync(string imageUri, string outputPath)
        {
            var res = await client.GetStreamAsync(new Uri(imageUri));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            using (var outputStream = new FileStream(outputPath, FileMode.CreateNew))
            {
                await res.CopyToAsync(outputStream);
            }
        }
    }
}
