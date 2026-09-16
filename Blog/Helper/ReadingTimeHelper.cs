using System;
using System.Text.RegularExpressions;

namespace Blog.Helper;

public static class ReadingTimeHelper
{
    private const int WordsPerMinute = 200;

    public static int CalculateReadingTime(string? htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent)) return 1;

        var plainText = System.Net.WebUtility.HtmlDecode(Regex.Replace(htmlContent, "<.*?>", string.Empty));
        var wordCount = plainText.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        return Math.Max(1, (int)Math.Ceiling((double)wordCount / WordsPerMinute));
    }
}
