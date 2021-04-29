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

        public static string GenerateThumbnailPath(string imageUri)
        {
            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var extension = Path.GetExtension(imageUri);
            return Path.Combine(ThumbnailDir, $"{timestamp}{extension}");
        }

        public async static Task<string> DownloadToThumbnailDirectoryAsync(string imageUri)
        {
            var outputPath = GenerateThumbnailPath(imageUri);
            await DownloadAsync(imageUri, outputPath);
            return outputPath;
        }

        private async static Task DownloadAsync(string sourceUri, string outputPath)
        {
            var res = await client.GetStreamAsync(new Uri(sourceUri));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            using (var outputStream = new FileStream(outputPath, FileMode.CreateNew))
            {
                await res.CopyToAsync(outputStream);
            }
        }

        public async static Task<string> CopyToThumbnailDirectoryAsync(string imagePath)
        {
            var outputPath = GenerateThumbnailPath(imagePath);
            await CopyAsync(imagePath, outputPath);
            return outputPath;
        }

        private async static Task CopyAsync(string sourcePath, string outputPath)
        {
            using (var sourceStream = File.Open(sourcePath, FileMode.Open))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                using (var outputStream = new FileStream(outputPath, FileMode.CreateNew))
                {
                    await sourceStream.CopyToAsync(outputStream);
                }
            }
        }
    }
}
