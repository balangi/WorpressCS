public static class StringExtensions
{
    public static string InsertHookedBlocks(this string content, string hookName)
    {
        // Simulate inserting blocks/hooks into content
        return $"<!-- wp-hook: {hookName} -->{content}<!-- /wp-hook -->";
    }
}