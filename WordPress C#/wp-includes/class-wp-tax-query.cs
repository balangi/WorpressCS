using System;
using System.Collections.Generic;
using System.Linq;

public class TaxQueryService
{
    private readonly AppDbContext _context;

    public TaxQueryService(AppDbContext context)
    {
        _context = context;
    }

    // اجرای Tax Query
    public IQueryable<int> ExecuteTaxQuery(List<TaxQueryClause> queries, string relation = "AND")
    {
        var query = _context.TermRelationships.AsQueryable();

        if (relation.Equals("AND", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var clause in queries)
            {
                query = ApplyClause(query, clause);
            }
        }
        else if (relation.Equals("OR", StringComparison.OrdinalIgnoreCase))
        {
            var combinedQuery = queries.Select(clause => ApplyClause(_context.TermRelationships, clause));
            query = combinedQuery.Aggregate((q1, q2) => q1.Union(q2));
        }

        return query.Select(tr => tr.ObjectId).Distinct();
    }

    // اعمال شرط‌های Clause
    private IQueryable<TermRelationship> ApplyClause(IQueryable<TermRelationship> query, TaxQueryClause clause)
    {
        if (clause.Terms == null || clause.Terms.Count == 0)
        {
            return query;
        }

        switch (clause.Operator.ToUpper())
        {
            case "IN":
                return query.Where(tr => clause.Terms.Contains(tr.TermId));

            case "NOT IN":
                return query.Where(tr => !clause.Terms.Contains(tr.TermId));

            case "EXISTS":
                return query.Where(tr => tr.TermId > 0); // همیشه وجود دارد

            case "NOT EXISTS":
                return query.Where(tr => tr.TermId <= 0); // هیچ Term مرتبطی ندارد

            default:
                throw new ArgumentException($"Unsupported operator: {clause.Operator}");
        }
    }

    // تبدیل Field به Term ID
    public List<int> TransformQuery(List<string> terms, string field, string taxonomy)
    {
        if (field.Equals("slug", StringComparison.OrdinalIgnoreCase))
        {
            return _context.Terms
                .Where(t => t.Taxonomy.Name == taxonomy && terms.Contains(t.Slug))
                .Select(t => t.Id)
                .ToList();
        }
        else if (field.Equals("name", StringComparison.OrdinalIgnoreCase))
        {
            return _context.Terms
                .Where(t => t.Taxonomy.Name == taxonomy && terms.Contains(t.Name))
                .Select(t => t.Id)
                .ToList();
        }
        else
        {
            return terms.Select(int.Parse).ToList();
        }
    }
}

// مدل Clause
public class TaxQueryClause
{
    public string Taxonomy { get; set; }
    public List<int> Terms { get; set; }
    public string Field { get; set; }
    public string Operator { get; set; }
    public bool IncludeChildren { get; set; }
}