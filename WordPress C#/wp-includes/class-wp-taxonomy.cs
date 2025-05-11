using System;
using System.Collections.Generic;
using System.Linq;

public class TaxonomyService
{
    private readonly AppDbContext _context;

    public TaxonomyService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک Taxonomy جدید
    public bool RegisterTaxonomy(string name, string label, string description, Dictionary<string, object> args)
    {
        if (_context.Taxonomies.Any(t => t.Name == name))
        {
            Console.WriteLine($"Taxonomy with name '{name}' already exists.");
            return false;
        }

        var taxonomy = new Taxonomy
        {
            Name = name,
            Label = label,
            Description = description,
            IsPublic = GetArgValue<bool>(args, "public", true),
            IsPubliclyQueryable = GetArgValue<bool>(args, "publicly_queryable", true),
            IsHierarchical = GetArgValue<bool>(args, "hierarchical", false),
            ShowInMenu = GetArgValue<bool>(args, "show_in_menu", true),
            ShowInNavMenus = GetArgValue<bool>(args, "show_in_nav_menus", true),
            ShowTagCloud = GetArgValue<bool>(args, "show_tagcloud", true),
            ShowAdminColumn = GetArgValue<bool>(args, "show_admin_column", false),
            MetaBoxCallback = GetArgValue<string>(args, "meta_box_cb", null),
            RewriteSlug = GetArgValue<string>(args, "rewrite_slug", null),
            QueryVar = GetArgValue<string>(args, "query_var", null)
        };

        _context.Taxonomies.Add(taxonomy);
        _context.SaveChanges();
        return true;
    }

    // افزودن داده‌های متا به یک Taxonomy
    public bool AddMeta(int taxonomyId, string metaKey, string metaValue)
    {
        var taxonomy = _context.Taxonomies.FirstOrDefault(t => t.Id == taxonomyId);
        if (taxonomy == null)
        {
            Console.WriteLine($"Taxonomy with ID '{taxonomyId}' not found.");
            return false;
        }

        var meta = new TaxonomyMeta
        {
            TaxonomyId = taxonomyId,
            MetaKey = metaKey,
            MetaValue = metaValue
        };

        _context.TaxonomyMeta.Add(meta);
        _context.SaveChanges();
        return true;
    }

    // دریافت تمام Taxonomy‌ها
    public List<Taxonomy> GetAllTaxonomies()
    {
        return _context.Taxonomies
            .Include(t => t.MetaData)
            .ToList();
    }

    // دریافت Taxonomy بر اساس نام
    public Taxonomy GetTaxonomyByName(string name)
    {
        return _context.Taxonomies
            .Include(t => t.MetaData)
            .FirstOrDefault(t => t.Name == name);
    }

    // حذف یک Taxonomy
    public bool DeleteTaxonomy(string name)
    {
        var taxonomy = _context.Taxonomies.FirstOrDefault(t => t.Name == name);
        if (taxonomy == null)
        {
            Console.WriteLine($"Taxonomy with name '{name}' not found.");
            return false;
        }

        _context.Taxonomies.Remove(taxonomy);
        _context.SaveChanges();
        return true;
    }

    // کمک‌کننده برای دریافت مقادیر از دیکشنری
    private T GetArgValue<T>(Dictionary<string, object> args, string key, T defaultValue)
    {
        if (args.ContainsKey(key) && args[key] is T value)
        {
            return value;
        }
        return defaultValue;
    }
}