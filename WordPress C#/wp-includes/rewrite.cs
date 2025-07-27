using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RewriteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RewriteService> _logger;

    public RewriteService(ApplicationDbContext context, ILogger<RewriteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Adds a rewrite rule.
    /// </summary>
    public void AddRewriteRule(string regexPattern, string query, string priority = "bottom")
    {
        if (string.IsNullOrEmpty(regexPattern) || string.IsNullOrEmpty(query))
        {
            throw new ArgumentException("Regex pattern and query cannot be empty.");
        }

        var rule = new RewriteRule
        {
            RegexPattern = regexPattern,
            Query = query,
            Priority = priority
        };

        _context.RewriteRules.Add(rule);
        _context.SaveChanges();
    }

    /// <summary>
    /// Retrieves all rewrite rules.
    /// </summary>
    public List<RewriteRule> GetRewriteRules()
    {
        return _context.RewriteRules.ToList();
    }

    /// <summary>
    /// Adds an endpoint.
    /// </summary>
    public void AddEndpoint(string name, string places, string queryVar = null)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(places))
        {
            throw new ArgumentException("Name and places cannot be empty.");
        }

        var endpoint = new Endpoint
        {
            Name = name,
            Places = places,
            QueryVar = queryVar
        };

        _context.Endpoints.Add(endpoint);
        _context.SaveChanges();
    }

    /// <summary>
    /// Removes an endpoint.
    /// </summary>
    public bool RemoveEndpoint(string name)
    {
        var endpoint = _context.Endpoints.FirstOrDefault(e => e.Name == name);
        if (endpoint == null)
        {
            return false;
        }

        _context.Endpoints.Remove(endpoint);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Adds a permalink structure.
    /// </summary>
    public void AddPermastruct(string name, string structure, Dictionary<string, object> args = null)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(structure))
        {
            throw new ArgumentException("Name and structure cannot be empty.");
        }

        var permastruct = new Permastruct
        {
            Name = name,
            Structure = structure,
            Args = args ?? new Dictionary<string, object>()
        };

        _context.Permastructs.Add(permastruct);
        _context.SaveChanges();
    }

    /// <summary>
    /// Removes a permalink structure.
    /// </summary>
    public bool RemovePermastruct(string name)
    {
        var permastruct = _context.Permastructs.FirstOrDefault(p => p.Name == name);
        if (permastruct == null)
        {
            return false;
        }

        _context.Permastructs.Remove(permastruct);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Flushes rewrite rules.
    /// </summary>
    public void FlushRewriteRules(bool hard = true)
    {
        _logger.LogInformation(hard ? "Hard flushing rewrite rules." : "Soft flushing rewrite rules.");

        // Simulate flushing logic here
        _context.RewriteRules.RemoveRange(_context.RewriteRules);
        _context.SaveChanges();

        // Optionally, recreate default rules
        AddDefaultRewriteRules();
    }

    /// <summary>
    /// Adds default rewrite rules.
    /// </summary>
    private void AddDefaultRewriteRules()
    {
        AddRewriteRule("^([^/]+)/?$", "index.php?name=$1");
        AddRewriteRule("^category/([^/]+)/?$", "index.php?category_name=$1");
        AddRewriteRule("^tag/([^/]+)/?$", "index.php?tag=$1");
    }
}