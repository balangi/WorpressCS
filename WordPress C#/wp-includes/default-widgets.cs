public abstract class Widget
{
    public string IdBase { get; set; }
    public string Name { get; set; }

    public Dictionary<string, object> WidgetOptions { get; set; } = new();
    public Dictionary<string, object> ControlOptions { get; set; } = new();

    public int Number { get; set; } = -1;
    public string Id => $"{IdBase}-{Number}";

    public virtual void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance)
    {
        Console.WriteLine($"Widget '{Name}' is not implemented.");
    }

    public virtual Dictionary<string, object> Update(Dictionary<string, object> newInstance, Dictionary<string, object> oldInstance)
    {
        return newInstance;
    }

    public virtual string Form(Dictionary<string, object> instance)
    {
        return "<p>This widget has no options.</p>";
    }
}