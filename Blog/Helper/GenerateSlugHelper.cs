using System.Text.RegularExpressions;

namespace Blog.Helper;

public static class GenerateSlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        text = text.ToLowerInvariant().Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");

        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-").Trim('-');

        return Regex.Replace(text, @"-+", "-");
    }
}
