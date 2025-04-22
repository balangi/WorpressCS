// BookmarkService.cs
public class BookmarkService
{
    private readonly ApplicationDbContext _context;

    public BookmarkService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> WalkBookmarksAsync(IEnumerable<Bookmark> bookmarks, BookmarkOptions options)
    {
        var output = new StringBuilder();

        foreach (var bookmark in bookmarks)
        {
            output.Append(options.Before);

            if (options.ShowUpdated && bookmark.RecentlyUpdated)
            {
                output.Append("<em>");
            }

            var theLink = !string.IsNullOrEmpty(bookmark.LinkUrl) ? bookmark.LinkUrl : "#";
            var desc = WebUtility.HtmlEncode(bookmark.LinkDescription ?? "");
            var name = WebUtility.HtmlEncode(bookmark.LinkName ?? "");
            var title = desc;

            if (options.ShowUpdated && bookmark.LinkUpdated > DateTime.MinValue)
            {
                title += $" (آخرین به‌روزرسانی: {bookmark.LinkUpdated:yyyy/MM/dd})";
            }

            var alt = $" alt=\"{name}{(options.ShowDescription ? " " + title : "")}\"";
            var titleAttr = !string.IsNullOrEmpty(title) ? $" title=\"{title}\"" : "";
            var rel = !string.IsNullOrEmpty(bookmark.LinkRel) ? $" rel=\"{WebUtility.HtmlEncode(bookmark.LinkRel)}\"" : "";
            var target = !string.IsNullOrEmpty(bookmark.LinkTarget) ? $" target=\"{bookmark.LinkTarget}\"" : "";

            output.Append($"<a href=\"{theLink}\"{rel}{titleAttr}{target}>");
            output.Append(options.LinkBefore);

            if (!string.IsNullOrEmpty(bookmark.LinkImage) && options.ShowImages)
            {
                if (bookmark.LinkImage.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    output.Append($"<img src=\"{bookmark.LinkImage}\"{alt}{titleAttr} />");
                }
                else
                {
                    output.Append($"<img src=\"{options.SiteUrl}{bookmark.LinkImage}\"{alt}{titleAttr} />");
                }

                if (options.ShowName)
                {
                    output.Append($" {name}");
                }
            }
            else
            {
                output.Append(name);
            }

            output.Append(options.LinkAfter);
            output.Append("</a>");

            if (options.ShowUpdated && bookmark.RecentlyUpdated)
            {
                output.Append("</em>");
            }

            if (options.ShowDescription && !string.IsNullOrEmpty(desc))
            {
                output.Append(options.Between + desc);
            }

            if (options.ShowRating)
            {
                output.Append(options.Between + bookmark.LinkRating.ToString());
            }

            output.Append(options.After + "\n");
        }

        return output.ToString();
    }

    public async Task<string> ListBookmarksAsync(BookmarkOptions options)
    {
        var output = new StringBuilder();

        if (options.Categorize)
        {
            var categoriesQuery = _context.BookmarkCategories.AsQueryable();

            if (!string.IsNullOrEmpty(options.CategoryName))
            {
                categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(options.CategoryName));
            }

            if (options.CategoryIds?.Any() == true)
            {
                categoriesQuery = categoriesQuery.Where(c => options.CategoryIds.Contains(c.Id));
            }

            if (options.ExcludeCategoryIds?.Any() == true)
            {
                categoriesQuery = categoriesQuery.Where(c => !options.ExcludeCategoryIds.Contains(c.Id));
            }

            categoriesQuery = options.CategoryOrderBy switch
            {
                "name" => options.CategoryOrder == "DESC" ? 
                    categoriesQuery.OrderByDescending(c => c.Name) : 
                    categoriesQuery.OrderBy(c => c.Name),
                _ => categoriesQuery
            };

            var categories = await categoriesQuery.ToListAsync();

            foreach (var category in categories)
            {
                var bookmarks = await GetBookmarksQuery(options, category.Id).ToListAsync();

                if (!bookmarks.Any())
                {
                    continue;
                }

                output.Append(options.CategoryBefore
                    .Replace("%id", $"linkcat-{category.Id}")
                    .Replace("%class", options.Class));

                output.Append(options.TitleBefore);
                output.Append(category.Name); // می‌توانید فیلتر اعمال کنید
                output.Append(options.TitleAfter);
                output.Append("\n\t<ul class='xoxo blogroll'>\n");
                output.Append(await WalkBookmarksAsync(bookmarks, options));
                output.Append("\n\t</ul>\n");
                output.Append(options.CategoryAfter + "\n");
            }
        }
        else
        {
            var bookmarks = await GetBookmarksQuery(options).ToListAsync();

            if (bookmarks.Any())
            {
                if (!string.IsNullOrEmpty(options.TitleLi))
                {
                    output.Append(options.CategoryBefore
                        .Replace("%id", $"linkcat-{options.CategoryIds?.FirstOrDefault()}")
                        .Replace("%class", options.Class));

                    output.Append(options.TitleBefore);
                    output.Append(options.TitleLi);
                    output.Append(options.TitleAfter);
                    output.Append("\n\t<ul class='xoxo blogroll'>\n");
                    output.Append(await WalkBookmarksAsync(bookmarks, options));
                    output.Append("\n\t</ul>\n");
                    output.Append(options.CategoryAfter + "\n");
                }
                else
                {
                    output.Append(await WalkBookmarksAsync(bookmarks, options));
                }
            }
        }

        return output.ToString();
    }

    private IQueryable<Bookmark> GetBookmarksQuery(BookmarkOptions options, int? categoryId = null)
    {
        var query = _context.Bookmarks
            .Include(b => b.Category)
            .AsQueryable();

        if (options.HideInvisible)
        {
            query = query.Where(b => b.IsVisible);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId);
        }
        else if (options.CategoryIds?.Any() == true)
        {
            query = query.Where(b => options.CategoryIds.Contains(b.CategoryId));
        }

        if (options.ExcludeCategoryIds?.Any() == true)
        {
            query = query.Where(b => !options.ExcludeCategoryIds.Contains(b.CategoryId));
        }

        query = options.OrderBy switch
        {
            "name" => options.Order == "DESC" ? 
                query.OrderByDescending(b => b.LinkName) : 
                query.OrderBy(b => b.LinkName),
            "id" => options.Order == "DESC" ? 
                query.OrderByDescending(b => b.Id) : 
                query.OrderBy(b => b.Id),
            "updated" => options.Order == "DESC" ? 
                query.OrderByDescending(b => b.LinkUpdated) : 
                query.OrderBy(b => b.LinkUpdated),
            _ => query
        };

        if (options.Limit > 0)
        {
            query = query.Take(options.Limit);
        }

        return query;
    }
}

// BookmarkOptions.cs
public class BookmarkOptions
{
    public string OrderBy { get; set; } = "name";
    public string Order { get; set; } = "ASC";
    public int Limit { get; set; } = -1;
    public List<int> CategoryIds { get; set; }
    public List<int> ExcludeCategoryIds { get; set; }
    public string CategoryName { get; set; }
    public bool HideInvisible { get; set; } = true;
    public bool ShowUpdated { get; set; }
    public bool Echo { get; set; } = true;
    public bool Categorize { get; set; } = true;
    public string TitleLi { get; set; } = "Bookmarks";
    public string TitleBefore { get; set; } = "<h2>";
    public string TitleAfter { get; set; } = "</h2>";
    public string CategoryOrderBy { get; set; } = "name";
    public string CategoryOrder { get; set; } = "ASC";
    public string Class { get; set; } = "linkcat";
    public string CategoryBefore { get; set; } = "<li id=\"%id\" class=\"%class\">";
    public string CategoryAfter { get: set; } = "</li>";
    public string Before { get; set; } = "<li>";
    public string After { get; set; } = "</li>";
    public string Between { get; set; } = "\n";
    public bool ShowDescription { get; set; }
    public bool ShowImages { get; set; } = true;
    public bool ShowName { get; set; }
    public bool ShowRating { get; set; }
    public string LinkBefore { get; set; } = "";
    public string LinkAfter { get; set; } = "";
    public string SiteUrl { get; set; }
}