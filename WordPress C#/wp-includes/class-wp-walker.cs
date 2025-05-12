public abstract class Walker<T> where T : TreeElement
{
    public abstract void StartLevel(StringBuilder output, int depth, object[] args);
    public abstract void EndLevel(StringBuilder output, int depth, object[] args);
    public abstract void StartElement(StringBuilder output, T element, int depth, object[] args, bool hasChildren = false);
    public abstract void EndElement(StringBuilder output, T element, int depth, object[] args);

    protected virtual void DisplayElement(
        StringBuilder output,
        T element,
        List<T> elements,
        Dictionary<int, List<T>> childrenMap,
        int maxDepth,
        int depth,
        object[] args)
    {
        if (element == null) return;

        var hasChildren = childrenMap.ContainsKey(element.Id);
        StartElement(output, element, depth, args, hasChildren);

        if (maxDepth <= 0 || depth < maxDepth)
        {
            if (hasChildren)
            {
                StartLevel(output, depth, args);
                foreach (var child in childrenMap[element.Id])
                {
                    DisplayElement(output, child, elements, childrenMap, maxDepth, depth + 1, args);
                }
                EndLevel(output, depth, args);
            }
        }

        EndElement(output, element, depth, args);
    }

    public string Walk(List<T> elements, int maxDepth, params object[] args)
    {
        var output = new StringBuilder();
        if (elements == null || elements.Count == 0) return output.ToString();

        var rootElements = elements.Where(e => e.ParentId == null).ToList();
        var childrenMap = elements
            .Where(e => e.ParentId.HasValue)
            .GroupBy(e => e.ParentId.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var root in rootElements)
        {
            DisplayElement(output, root, elements, childrenMap, maxDepth, 0, args);
        }

        return output.ToString();
    }
}