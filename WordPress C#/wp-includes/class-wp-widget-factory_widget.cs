public abstract class Widget : IWidget
{
    public string IdBase { get; set; }
    public string Name { get; set; }

    public abstract void Widget(Dictionary<string, object> args, Dictionary<string, object> instance);
    
    public virtual Dictionary<string, object> Update(Dictionary<string, object> newInstance, Dictionary<string, object> oldInstance)
    {
        return newInstance;
    }

    public virtual string Form(Dictionary<string, object> instance)
    {
        return "<p>There are no options for this widget.</p>";
    }
}