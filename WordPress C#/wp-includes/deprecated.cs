public static class DeprecatedExtensions
{
    public static string GetTheAuthorLogin(this IHtmlHelper htmlHelper)
    {
        throw new NotSupportedException("This method is deprecated. Use 'the_author_meta(\"login\")' instead.");
    }

    public static string GetTheAuthorUrl(this IHtmlHelper htmlHelper)
    {
        throw new NotSupportedException("This method is deprecated. Use 'the_author_meta(\"url\")' instead.");
    }

    public static string GetTheAuthorId(this IHtmlHelper htmlHelper)
    {
        throw new NotSupportedException("This method is deprecated. Use 'the_author_meta(\"ID\")' instead.");
    }

    public static string GetPostData(int postId)
    {
        throw new NotSupportedException("This method is deprecated. Use 'get_post()' instead.");
    }

    public static string TheBlockTemplateSkipLink()
    {
        throw new NotSupportedException("This function is deprecated. Use 'wp_enqueue_block_template_skip_link()' instead.");
    }

    public static string ForceSslLogin(string force = null)
    {
        throw new NotSupportedException("This function is deprecated. Use 'force_ssl_admin()' instead.");
    }

    public static string GetCategoryRssLink(bool display = false, int categoryId = 1)
    {
        throw new NotSupportedException("This function is deprecated. Use 'get_category_feed_link()' instead.");
    }

    public static string GetAuthorRssLink(bool display = false, int authorId = 1)
    {
        throw new NotSupportedException("This function is deprecated. Use 'get_author_feed_link()' instead.");
    }

    public static string CommentsRss()
    {
        throw new NotSupportedException("This function is deprecated. Use 'get_post_comments_feed_link()' instead.");
    }

    public static string Createuser(string username, string password, string email)
    {
        throw new NotSupportedException("This function is deprecated. Use 'wp_create_user()' instead.");
    }
}