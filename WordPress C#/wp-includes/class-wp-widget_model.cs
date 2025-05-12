public abstract class Widget
{
    public string IdBase { get; set; } // مثل id_base در PHP
    public string Name { get; set; }   // نام ویجت
    public string OptionName => $"widget_{IdBase}"; // option_name
    public string AltOptionName => $"sidebars_widgets[{IdBase}]"; // optional

    public Dictionary<string, object> WidgetOptions { get; set; } = new();
    public Dictionary<string, object> ControlOptions { get; set; } = new();

    public int Number { get; set; } = -1; // عدد منحصر به فرد برای هر نمونه
    public string Id => $"{IdBase}-{Number}";

    public bool Updated { get; set; } = false;

    // باید override بشود
    public abstract void Widget(IHtmlHelper htmlHelper, Dictionary<string, object> args, Dictionary<string, object> instance);
    
    // اختیاری override
    public virtual Dictionary<string, object> Update(Dictionary<string, object> newInstance, Dictionary<string, object> oldInstance)
    {
        return newInstance;
    }

    public virtual string Form(Dictionary<string, object> instance)
    {
        return "<p>There are no options for this widget.</p>";
    }
}

public class WidgetSettings
{
    public int Id { get; set; }
    public string WidgetId { get; set; } // "recent-posts-1"
    public Dictionary<string, object> Settings { get; set; } = new();
}