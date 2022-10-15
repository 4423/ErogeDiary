using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models;
public static class ThumbnailHelper
{
    private static readonly HttpClient client;
    private static readonly int SHORT_SIDE_SIZE_PX = 200;


    static ThumbnailHelper()
    {
        client = new HttpClient();

        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ThumbnailDir = Path.Combine(currentPath!, "thumbnails");
        Directory.CreateDirectory(ThumbnailDir);
    }

    public static string ThumbnailDir { get; private set; }

    public static string GenerateThumbnailPath(string imageUri)
    {
        var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var extension = Path.GetExtension(imageUri);
        return Path.Combine(ThumbnailDir, $"{timestamp}{extension}");
    }

    public async static Task<string> DownloadAndResizeAsync(string imageUrl)
    {
        var thumbnailPath = GenerateThumbnailPath(imageUrl);

        using var responseStream = await client.GetStreamAsync(new Uri(imageUrl));
        await ResizAndWrite(responseStream, thumbnailPath);

        return thumbnailPath;
    }

    public async static Task<string> CopyAndResize(string imagePath)
    {
        var thumbnailPath = GenerateThumbnailPath(imagePath);

        using var fileStream = File.Open(imagePath, FileMode.Open);
        await ResizAndWrite(fileStream, thumbnailPath);

        return thumbnailPath;
    }

    private async static Task ResizAndWrite(Stream imageStream, string thumbnailPath)
    {
        using var image = new MagickImage(imageStream);
        ResizeBasedOnShortSide(image);
        await image.WriteAsync(thumbnailPath);
    }

    private static void ResizeBasedOnShortSide(MagickImage image)
    {
        if (image.Width > image.Height)
        {
            image.Resize(width: 0, height: SHORT_SIDE_SIZE_PX);
        }
        else
        {
            image.Resize(width: SHORT_SIDE_SIZE_PX, height: 0);
        }
    }
}
