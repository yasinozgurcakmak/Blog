using Microsoft.AspNetCore.Http;

namespace Blog.Helper;

public static class UploadPathHelper
{
    private const string UploadsSrc = "src=\"/uploads/";

    public static string AddPathBase(string? htmlContent, PathString pathBase)
    {
        if (string.IsNullOrEmpty(htmlContent) || !pathBase.HasValue) return htmlContent ?? string.Empty;
        return htmlContent.Replace(UploadsSrc, $"src=\"{pathBase}/uploads/");
    }

    public static string RemovePathBase(string htmlContent, PathString pathBase)
    {
        if (string.IsNullOrEmpty(htmlContent) || !pathBase.HasValue) return htmlContent;
        return htmlContent.Replace($"src=\"{pathBase}/uploads/", UploadsSrc);
    }
}
