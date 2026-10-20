using Ganss.Xss;
namespace Blog.Helper;

public static class HtmlSanitizerHelper
{
    private static readonly HtmlSanitizer Sanitizer = new();

    static HtmlSanitizerHelper()
    {
        Sanitizer.AllowedAttributes.Add("class");
        Sanitizer.AllowedAttributes.Add("style");
        Sanitizer.AllowedAttributes.Add("target");
        Sanitizer.FilterUrl += (_, e) =>
        {
            if (e.SanitizedUrl is null
                && e.Tag.NodeName.Equals("IMG", StringComparison.OrdinalIgnoreCase)
                && e.OriginalUrl.TrimStart().StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                e.SanitizedUrl = e.OriginalUrl;
            }
        };
    }

    public static string Sanitize(string? html) => string.IsNullOrWhiteSpace(html) ? string.Empty : Sanitizer.Sanitize(html);
}
