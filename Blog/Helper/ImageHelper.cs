using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Helper;

public static class ImageHelper
{
    private static readonly WebpEncoder WebpEncoderSettings = new() { Quality = 80 };
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/pjpeg", "image/x-png"];

    public static async Task<string> SaveImageAsWebpAsync(IFormFile file, string folderPath, string fileNamePrefix = "thumb")
    {
        Directory.CreateDirectory(folderPath);

        var fileName = $"{fileNamePrefix}_{Guid.NewGuid().ToString("N")[..8]}.webp";
        var savePath = Path.Combine(folderPath, fileName);

        using var inputStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(inputStream);
        await image.SaveAsync(savePath, WebpEncoderSettings);

        return fileName;
    }

    public static string SaveBytesAsWebp(byte[] imageBytes, string folderPath, string fileNamePrefix = "content")
    {
        Directory.CreateDirectory(folderPath);

        var fileName = $"{fileNamePrefix}_{Guid.NewGuid().ToString("N")[..8]}.webp";
        var savePath = Path.Combine(folderPath, fileName);

        using var image = Image.Load(imageBytes);
        image.Save(savePath, WebpEncoderSettings);

        return fileName;
    }

    public static bool IsValidImage(IFormFile? file, out string? errorMessage, long maxFileSizeBytes = 5 * 1024 * 1024)
    {
        errorMessage = null;
        if (file is null || file.Length == 0) return true;

        if (file.Length > maxFileSizeBytes)
        {
            errorMessage = $"Dosya boyutu en fazla {maxFileSizeBytes / (1024 * 1024)}MB olabilir.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            errorMessage = "Yalnızca JPG, JPEG, PNG veya WEBP formatında görseller yüklenebilir.";
            return false;
        }

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            errorMessage = "Geçersiz görsel dosya türü.";
            return false;
        }

        return true;
    }
}
