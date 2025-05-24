public class SearchWidget : Widget
{
    public SearchWidget()
    {
        IdBase = "search";
        Name = "Search";
    }

    public override void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance)
    {
        Console.WriteLine(@"
            <form method='get' action='/search'>
                <input type='text' name='q' placeholder='Search...' />
                <button type='submit'>🔍</button>
            </form>");
    }
}