using System;
using System.Linq;

public class TermService
{
    private readonly AppDbContext _context;

    public TermService(AppDbContext context)
    {
        _context = context;
    }

    // دریافت یک Term بر اساس شناسه
    public Term GetTermById(int termId, string taxonomy = null)
    {
        var term = _context.Terms
            .Include(t => t.TermTaxonomy)
            .FirstOrDefault(t => t.TermId == termId);

        if (term == null)
        {
            return null;
        }

        // بررسی Taxonomy
        if (!string.IsNullOrEmpty(taxonomy))
        {
            if (term.TermTaxonomy?.Taxonomy != taxonomy)
            {
                throw new InvalidOperationException($"Term with ID {termId} does not belong to the specified taxonomy.");
            }
        }

        return term;
    }

    // سانیتایز کردن داده‌ها
    public Term SanitizeTerm(Term term, string filter = "raw")
    {
        if (filter.Equals("raw", StringComparison.OrdinalIgnoreCase))
        {
            return term;
        }

        // اعمال سانیتایز بسته به نوع فیلتر
        switch (filter.ToLower())
        {
            case "edit":
            case "display":
                term.Name = SanitizeInput(term.Name);
                term.Description = SanitizeInput(term.Description);
                break;
            default:
                throw new ArgumentException($"Unsupported filter type: {filter}");
        }

        return term;
    }

    // تبدیل Term به آرایه
    public object ToArray(Term term)
    {
        return new
        {
            term.TermId,
            term.Name,
            term.Slug,
            term.TermGroup,
            term.TermTaxonomyId,
            term.Taxonomy,
            term.Description,
            term.Parent,
            term.Count
        };
    }

    // سانیتایز ورودی
    private string SanitizeInput(string input)
    {
        // می‌توانید از کتابخانه‌هایی مانند HtmlSanitizer استفاده کنید
        return input?.Trim();
    }
}