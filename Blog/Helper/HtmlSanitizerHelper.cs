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
        Sanitizer.AllowedSchemes.Add("data");
    }

    public static string Sanitize(string? html) => string.IsNullOrWhiteSpace(html) ? string.Empty : Sanitizer.Sanitize(html);
}
