using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Environment
{
    public class WP
    {
        /// <summary>
        /// Public query variables.
        /// </summary>
        public Dictionary<string, string> QueryVars { get; set; } = new Dictionary<string, string>();

        private readonly AppDbContext _dbContext;

        public WP(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Processes the query variables and sets up the environment.
        /// </summary>
        public void ProcessQueryVariables()
        {
            // Example: Load query variables from the database
            var queryVariables = _dbContext.QueryVariables.ToDictionary(q => q.Key, q => q.Value);

            foreach (var queryVar in queryVariables)
            {
                if (!QueryVars.ContainsKey(queryVar.Key))
                {
                    QueryVars[queryVar.Key] = queryVar.Value;
                }
            }

            // Perform additional processing if needed
            if (QueryVars.ContainsKey("error"))
            {
                Console.WriteLine($"Error detected: {QueryVars["error"]}");
                QueryVars.Remove("error");
            }

            // Trigger custom actions or filters
            TriggerActions();
        }

        /// <summary>
        /// Triggers custom actions or filters.
        /// </summary>
        private void TriggerActions()
        {
            // Simulate WordPress-style action hooks
            Console.WriteLine("Triggering 'wp' action...");
            // You can use a more advanced event system here
        }

        /// <summary>
        /// Adds or updates a query variable.
        /// </summary>
        /// <param name="key">The key of the query variable.</param>
        /// <param name="value">The value of the query variable.</param>
        public void AddOrUpdateQueryVariable(string key, string value)
        {
            if (QueryVars.ContainsKey(key))
            {
                QueryVars[key] = value;
            }
            else
            {
                QueryVars.Add(key, value);
            }

            // Save to database if needed
            var dbEntry = _dbContext.QueryVariables.FirstOrDefault(q => q.Key == key);
            if (dbEntry != null)
            {
                dbEntry.Value = value;
            }
            else
            {
                _dbContext.QueryVariables.Add(new QueryVariable { Key = key, Value = value });
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes a query variable.
        /// </summary>
        /// <param name="key">The key of the query variable to remove.</param>
        public void RemoveQueryVariable(string key)
        {
            if (QueryVars.ContainsKey(key))
            {
                QueryVars.Remove(key);
            }

            // Remove from database if needed
            var dbEntry = _dbContext.QueryVariables.FirstOrDefault(q => q.Key == key);
            if (dbEntry != null)
            {
                _dbContext.QueryVariables.Remove(dbEntry);
                _dbContext.SaveChanges();
            }
        }
    }
}