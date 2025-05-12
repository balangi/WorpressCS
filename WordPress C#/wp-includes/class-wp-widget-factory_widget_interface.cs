public interface IWidget
{
    string IdBase { get; }
    string Name { get; }

    void Widget(Dictionary<string, object> args, Dictionary<string, object> instance);
    Dictionary<string, object> Update(Dictionary<string, object> newInstance, Dictionary<string, object> oldInstance);
    string Form(Dictionary<string, object> instance);
}