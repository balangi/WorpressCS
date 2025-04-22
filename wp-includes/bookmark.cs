public class Bookmark
{
    public int LinkId { get; set; }
    public string LinkUrl { get; set; }
    public string LinkName { get; set; }
    public string LinkImage { get; set; }
    public string LinkTarget { get; set; }
    public List<int> LinkCategory { get; set; }
    public string LinkDescription { get; set; }
    public string LinkVisible { get; set; }
    public int LinkOwner { get; set; }
    public int LinkRating { get; set; }
    public DateTime LinkUpdated { get; set; }
    public string LinkRel { get; set; }
    public string LinkNotes { get; set; }
    public string LinkRss { get; set; }
}
public class Bookmark{
	public object GetBookmark(object bookmark, string output = "OBJECT", string filter = "raw")
{
    using (var context = new ApplicationDbContext())
    {
        Bookmark _bookmark = null;

        if (bookmark == null)
        {
            _bookmark = context.Bookmarks.FirstOrDefault();
        }
        else if (bookmark is Bookmark bookmarkObj)
        {
            _bookmark = bookmarkObj;
        }
        else if (int.TryParse(bookmark.ToString(), out int bookmarkId))
        {
            _bookmark = context.Bookmarks.FirstOrDefault(b => b.LinkId == bookmarkId);
        }

        if (_bookmark == null)
        {
            return null;
        }

        _bookmark = SanitizeBookmark(_bookmark, filter);

        switch (output.ToUpper())
        {
            case "OBJECT":
                return _bookmark;
            case "ARRAY_A":
                return _bookmark.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(_bookmark));
            case "ARRAY_N":
                return _bookmark.GetType().GetProperties().Select(p => p.GetValue(_bookmark)).ToList();
            default:
                return _bookmark;
        }
    }
}

public List<Bookmark> GetBookmarks(Dictionary<string, object> args)
{
    var cacheKey = GenerateCacheKey(args);

    if (_memoryCache.TryGetValue(cacheKey, out List<Bookmark> cachedBookmarks))
    {
        return cachedBookmarks;
    }

    using (var context = new ApplicationDbContext())
    {
        var query = context.Bookmarks.AsQueryable();

        if (args.ContainsKey("include") && args["include"] != null)
        {
            var includeIds = args["include"].ToString().Split(',').Select(int.Parse).ToList();
            query = query.Where(b => includeIds.Contains(b.LinkId));
        }

        if (args.ContainsKey("exclude") && args["exclude"] != null)
        {
            var excludeIds = args["exclude"].ToString().Split(',').Select(int.Parse).ToList();
            query = query.Where(b => !excludeIds.Contains(b.LinkId));
        }

        if (args.ContainsKey("category") && args["category"] != null)
        {
            var categoryIds = args["category"].ToString().Split(',').Select(int.Parse).ToList();
            query = query.Where(b => categoryIds.Any(cat => b.LinkCategory.Contains(cat)));
        }

        if (args.ContainsKey("search") && args["search"] != null)
        {
            var search = args["search"].ToString();
            query = query.Where(b =>
                b.LinkUrl.Contains(search) ||
                b.LinkName.Contains(search) ||
                b.LinkDescription.Contains(search));
        }

        var orderBy = args.ContainsKey("orderby") ? args["orderby"].ToString() : "name";
        var order = args.ContainsKey("order") ? args["order"].ToString() : "ASC";

        query = orderBy.ToLower() switch
        {
            "link_id" => order == "ASC" ? query.OrderBy(b => b.LinkId) : query.OrderByDescending(b => b.LinkId),
            "link_name" => order == "ASC" ? query.OrderBy(b => b.LinkName) : query.OrderByDescending(b => b.LinkName),
            _ => query.OrderBy(b => b.LinkName)
        };

        var limit = args.ContainsKey("limit") ? int.Parse(args["limit"].ToString()) : -1;
        if (limit > 0)
        {
            query = query.Take(limit);
        }

        var result = query.ToList();
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }
}

public Bookmark SanitizeBookmark(Bookmark bookmark, string context = "display")
{
    var fields = new List<string>
    {
        "LinkId", "LinkUrl", "LinkName", "LinkImage", "LinkTarget",
        "LinkCategory", "LinkDescription", "LinkVisible", "LinkOwner",
        "LinkRating", "LinkUpdated", "LinkRel", "LinkNotes", "LinkRss"
    };

    foreach (var field in fields)
    {
        var value = bookmark.GetType().GetProperty(field)?.GetValue(bookmark);
        if (value != null)
        {
            var sanitizedValue = SanitizeBookmarkField(field, value, bookmark.LinkId, context);
            bookmark.GetType().GetProperty(field)?.SetValue(bookmark, sanitizedValue);
        }
    }

    return bookmark;
}

private object SanitizeBookmarkField(string field, object value, int bookmarkId, string context)
{
    if (field == "LinkCategory" && value is List<int> categoryList)
    {
        return categoryList.Select(cat => Math.Abs(cat)).ToList();
    }

    if (field == "LinkVisible" && value is string visible)
    {
        return Regex.Replace(visible, "[^YNyn]", "");
    }

    if (context == "raw")
    {
        return value;
    }

    if (context == "edit")
    {
        value = value is string strValue ? HttpUtility.HtmlEncode(strValue) : value;
    }

    return value;
}

private readonly IMemoryCache _memoryCache;

public YourService(IMemoryCache memoryCache)
{
    _memoryCache = memoryCache;
}

private string GenerateCacheKey(Dictionary<string, object> args)
{
    return string.Join("_", args.Select(kvp => $"{kvp.Key}_{kvp.Value}"));
}
}