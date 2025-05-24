public static class SanitizeFilters
{
    public static string SanitizeTextField(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Remove HTML tags and decode entities
        var stripped = System.Net.WebUtility.HtmlDecode(Regex.Replace(input, "<.*?>", string.Empty));
        // Trim and remove extra spaces
        return Regex.Replace(stripped.Trim(), @"\s+", " ");
    }

    public static string Kses(string input)
    {
        // Placeholder for kses-like filtering (HTML sanitization)
        var allowedTags = new[] { "b", "i", "u", "strong", "em" };
        return Regex.Replace(input, @"<(/?)(\w+)", match =>
        {
            var tag = match.Groups[2].Value;
            if (allowedTags.Contains(tag.ToLower()))
            {
                return $"<{match.Groups[1].Value}{tag}";
            }
            return "";
        });
    }

    public static string WpSpecialChars(string input)
    {
        return System.Net.WebUtility.HtmlEncode(input);
    }
}