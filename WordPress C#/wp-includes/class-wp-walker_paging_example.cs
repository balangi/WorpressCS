public string PagedWalk(List<T> elements, int maxDepth, int pageNum, int perPage, params object[] args)
{
    var output = new StringBuilder();
    if (elements == null || elements.Count == 0) return output.ToString();

    var rootElements = elements.Where(e => e.ParentId == null).ToList();

    // صفحه‌بندی روی آیتم‌های روت
    var pagedRoots = rootElements.Skip((pageNum - 1) * perPage).Take(perPage).ToList();

    var childrenMap = elements
        .Where(e => e.ParentId.HasValue)
        .GroupBy(e => e.ParentId.Value)
        .ToDictionary(g => g.Key, g => g.ToList());

    foreach (var root in pagedRoots)
    {
        DisplayElement(output, root, elements, childrenMap, maxDepth, 0, args);
    }

    return output.ToString();
}