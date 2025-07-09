using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class MsNetworkService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public MsNetworkService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// Retrieves network data given a network ID or network object.
    /// </summary>
    public Network GetNetwork(int? networkId = null)
    {
        var currentNetwork = _cache.Get<Network>("current_network");

        if (!networkId.HasValue && currentNetwork != null)
        {
            return currentNetwork;
        }

        var network = _context.Networks.FirstOrDefault(n => n.Id == networkId);
        if (network != null)
        {
            _cache.Set("current_network", network);
        }

        return network;
    }

    /// <summary>
    /// Retrieves a list of networks based on query arguments.
    /// </summary>
    public List<Network> GetNetworks(Dictionary<string, object> args = null)
    {
        var query = _context.Networks.AsQueryable();

        if (args != null)
        {
            if (args.ContainsKey("id"))
            {
                query = query.Where(n => n.Id == (int)args["id"]);
            }

            if (args.ContainsKey("domain"))
            {
                query = query.Where(n => n.Domain.Contains((string)args["domain"]));
            }

            if (args.ContainsKey("path"))
            {
                query = query.Where(n => n.Path.Contains((string)args["path"]));
            }
        }

        return query.ToList();
    }

    /// <summary>
    /// Removes a network from the cache.
    /// </summary>
    public void CleanNetworkCache(List<int> networkIds)
    {
        foreach (var id in networkIds)
        {
            _cache.Remove($"network_{id}");
        }
    }

    /// <summary>
    /// Updates the network cache with new data.
    /// </summary>
    public void UpdateNetworkCache(List<Network> networks)
    {
        foreach (var network in networks)
        {
            _cache.Set($"network_{network.Id}", network);
        }
    }

    /// <summary>
    /// Primes the network cache by fetching non-cached networks from the database.
    /// </summary>
    public void PrimeNetworkCache(List<int> networkIds)
    {
        var cachedIds = _cache.Get<List<int>>("cached_network_ids") ?? new List<int>();
        var nonCachedIds = networkIds.Except(cachedIds).ToList();

        if (nonCachedIds.Any())
        {
            var freshNetworks = _context.Networks.Where(n => nonCachedIds.Contains(n.Id)).ToList();
            UpdateNetworkCache(freshNetworks);

            cachedIds.AddRange(nonCachedIds);
            _cache.Set("cached_network_ids", cachedIds);
        }
    }
}