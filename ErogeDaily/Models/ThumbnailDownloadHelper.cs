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

        public async static Task<Uri> DownloadAsync(Uri imageUri)
        {
            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var extension = Path.GetExtension(imageUri.AbsolutePath);
            var outputPath = Path.Combine(ThumbnailDir, $"{timestamp}{extension}");

            await DownloadCoreAsync(imageUri, outputPath);
            
            return new Uri(outputPath);
        }

        private async static Task DownloadCoreAsync(Uri imageUri, string outputPath)
        {
            var res = await client.GetStreamAsync(imageUri);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            using (var outputStream = new FileStream(outputPath, FileMode.CreateNew))
            {
                await res.CopyToAsync(outputStream);
            }
        }
    }
}
