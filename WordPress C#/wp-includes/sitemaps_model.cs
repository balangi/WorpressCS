using System;
using System.Collections.Generic;

public class SitemapProvider
{
    public string Name { get; set; }
    public Func<string, int, string> GetSitemapUrl { get; set; }
    public Dictionary<string, object> ObjectSubtypes { get; set; } = new Dictionary<string, object>();
}

public class SitemapServer
{
    public Dictionary<string, SitemapProvider> Providers { get; set; } = new Dictionary<string, SitemapProvider>();

    public void Init()
    {
        // Initialize the sitemap system here
    }

    public void AddProvider(string name, SitemapProvider provider)
    {
        Providers[name] = provider;
    }

    public SitemapProvider GetProvider(string name)
    {
        return Providers.ContainsKey(name) ? Providers[name] : null;
    }
}