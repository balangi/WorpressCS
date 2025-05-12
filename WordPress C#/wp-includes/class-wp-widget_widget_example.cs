public class RecentPostsWidget : Widget
{
    public RecentPostsWidget()
    {
        IdBase = "recent_posts";
        Name = "Recent Posts";
    }

    public override void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance)
    {
        var title = instance.ContainsKey("title") ? instance["title"].ToString() : "Recent Posts";

        Console.WriteLine($"<h3>{title}</h3>");
        Console.WriteLine("<ul>");

        // فرض کنید اینجا از EF یا API آخرین پست‌ها را گرفته‌ایم
        Console.WriteLine("<li><a href=\"/post1\">Post 1</a></li>");
        Console.WriteLine("<li><a href=\"/post2\">Post 2</a></li>");

        Console.WriteLine("</ul>");
    }

    public override Dictionary<string, object> Update(Dictionary<string, object> newInstance, Dictionary<string, object> oldInstance)
    {
        oldInstance["title"] = newInstance["title"];
        return oldInstance;
    }

    public override string Form(Dictionary<string, object> instance)
    {
        var title = instance.ContainsKey("title") ? instance["title"].ToString() : "Recent Posts";
        return $@"
            <p>
                <label>Title:</label>
                <input type='text' name='widget-{IdBase}[{Number}][title]' value='{title}' />
            </p>";
    }
}