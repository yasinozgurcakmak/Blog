using Microsoft.AspNetCore.Http;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Helper;

public static class ImageHelper
{
    private const int WebpQuality = 80;

    private const int MaxWebpDimension = 16383;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/pjpeg", "image/x-png"];

    public static async Task<string> SaveImageAsWebpAsync(IFormFile file, string folderPath, string fileNamePrefix = "thumb")
    {
        Directory.CreateDirectory(folderPath);

        var fileName = $"{fileNamePrefix}_{Guid.NewGuid().ToString("N")[..8]}.webp";
        var savePath = Path.Combine(folderPath, fileName);

        using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer);

        using var webp = EncodeAsWebp(buffer.ToArray());
        await using var output = File.Create(savePath);
        await using var webpStream = webp.AsStream();
        await webpStream.CopyToAsync(output);

        return fileName;
    }

    public static string SaveBytesAsWebp(byte[] imageBytes, string folderPath, string fileNamePrefix = "content")
    {
        Directory.CreateDirectory(folderPath);

        var fileName = $"{fileNamePrefix}_{Guid.NewGuid().ToString("N")[..8]}.webp";
        var savePath = Path.Combine(folderPath, fileName);

        using var webp = EncodeAsWebp(imageBytes);
        using var output = File.Create(savePath);
        webp.SaveTo(output);

        return fileName;
    }

    private static SKData EncodeAsWebp(byte[] imageBytes)
    {
        using var data = SKData.CreateCopy(imageBytes);
        using var codec = SKCodec.Create(data) ?? throw new InvalidDataException("Görsel okunamadı veya desteklenmeyen bir formatta.");

        if (codec.Info.Width > MaxWebpDimension || codec.Info.Height > MaxWebpDimension)
        {
            throw new InvalidDataException($"Görsel boyutu çok büyük; kenarlar en fazla {MaxWebpDimension} piksel olabilir.");
        }

        using var bitmap = SKBitmap.Decode(codec) ?? throw new InvalidDataException("Görsel okunamadı.");
        using var image = SKImage.FromBitmap(bitmap);
        return image.Encode(SKEncodedImageFormat.Webp, WebpQuality) ?? throw new InvalidOperationException("Görsel WebP formatına dönüştürülemedi.");
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
