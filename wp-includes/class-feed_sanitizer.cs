using System.Text.RegularExpressions;

public static class FeedSanitizer
{
    private static readonly Regex AllowedTags = new Regex(
        @"<(b|i|em|strong|a|ul|ol|li|p|br|div|span)[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public static string SanitizeContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        // حذف تگ‌های غیرمجاز
        var sanitized = AllowedTags.Replace(content, string.Empty);
        
        // پاکسازی ویژگی‌های خطرناک
        sanitized = Regex.Replace(
            sanitized,
            @"<(.*?)>",
            m => CleanAttributes(m.Groups[1].Value)
        );
        
        return sanitized;
    }

    private static string CleanAttributes(string tag)
    {
        return Regex.Replace(
            tag,
            @"(\w+)=[""']?((?:.(?![""']?\s+(?:\S+)=|\s*\/?[>""']))+.)[""']?",
            m => AllowedAttributes(m)
        );
    }

    private static string AllowedAttributes(Match match)
    {
        var attribute = match.Groups[1].Value.ToLower();
        return attribute switch
        {
            "href" or "title" or "class" => match.Value,
            _ => string.Empty
        };
    }
}