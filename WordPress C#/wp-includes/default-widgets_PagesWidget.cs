public class PagesWidget : Widget
{
    public PagesWidget()
    {
        IdBase = "pages";
        Name = "Pages";
    }

    public override void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance)
    {
        var pages = new List<string> { "Home", "About", "Contact" };

        Console.WriteLine("<ul>");
        foreach (var page in pages)
        {
            Console.WriteLine($"<li><a href='/{page.ToLower()}'>{page}</a></li>");
        }
        Console.WriteLine("</ul>");
    }
}