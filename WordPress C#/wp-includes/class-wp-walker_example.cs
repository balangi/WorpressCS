public class MenuItem : TreeElement { }

public class MenuWalker : Walker<MenuItem>
{
    public override void StartLevel(StringBuilder output, int depth, object[] args)
    {
        output.AppendLine("<ul>");
    }

    public override void EndLevel(StringBuilder output, int depth, object[] args)
    {
        output.AppendLine("</ul>");
    }

    public override void StartElement(StringBuilder output, MenuItem element, int depth, object[] args, bool hasChildren = false)
    {
        output.AppendLine($"<li>{element.Name}");
    }

    public override void EndElement(StringBuilder output, MenuItem element, int depth, object[] args)
    {
        output.AppendLine("</li>");
    }
}