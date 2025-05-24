public class RecentPostsWidget : Widget
{
    public RecentPostsWidget()
    {
        IdBase = "recent_posts";
        Name = "Recent Posts";
    }

    public override void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance)
    {
        var posts = new List<string> { "Post 1", "Post 2", "Post 3" };

        Console.WriteLine("<ul>");
        foreach (var post in posts)
        {
            Console.WriteLine($"<li><a href='/post/{post.Replace(" ", "-")}'>{post}</a></li>");
        }
        Console.WriteLine("</ul>");
    }
}