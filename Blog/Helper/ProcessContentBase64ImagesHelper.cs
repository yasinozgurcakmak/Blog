using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Blog.Helper;

public static class ProcessContentBase64ImagesHelper
{
    private static readonly Regex Base64ImageRegex = new(
        @"src=[""']data:image/(?<type>png|jpg|jpeg|gif|webp);base64,(?<data>[^""']+)[""']",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string ProcessContentBase64Images(string htmlContent, string slug, string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(htmlContent)) return htmlContent;

        var matches = Base64ImageRegex.Matches(htmlContent);
        if (matches.Count == 0) return htmlContent;

        var blogFolder = Path.Combine(webRootPath, "uploads", "blogs", slug);
        Directory.CreateDirectory(blogFolder);

        foreach (Match match in matches)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(match.Groups["data"].Value);
                var imageName = ImageHelper.SaveBytesAsWebp(imageBytes, blogFolder, "content");
                htmlContent = htmlContent.Replace(match.Value, $"src=\"/uploads/blogs/{slug}/{imageName}\"");
            }
            catch
            {
                // 
            }
        }

        return htmlContent;
    }
}
