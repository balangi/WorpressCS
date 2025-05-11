using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WordPress.Users
{
    /// <summary>
    /// Core class used for querying users.
    /// </summary>
    public class WP_User_Query
    {
        /// <summary>
        /// Query variables, after parsing.
        /// </summary>
        public Dictionary<string, object> QueryVars { get; private set; } = new();

        /// <summary>
        /// List of found user IDs or user objects.
        /// </summary>
        public List<object> Results { get; private set; }

        /// <summary>
        /// Total number of found users for the current query.
        /// </summary>
        public int TotalUsers { get; private set; }

        /// <summary>
        /// Metadata query container.
        /// </summary>
        public WP_Meta_Query MetaQuery { get; private set; }

        /// <summary>
        /// The SQL query used to fetch matching users.
        /// </summary>
        public string Request { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="query">Optional. The query variables.</param>
        public WP_User_Query(Dictionary<string, object> query = null)
        {
            if (query != null && query.Any())
            {
                PrepareQuery(query);
                Query();
            }
        }

        /// <summary>
        /// Prepares the query by filling in missing query variables with default values.
        /// </summary>
        /// <param name="query">Query variables.</param>
        public void PrepareQuery(Dictionary<string, object> query)
        {
            QueryVars = FillQueryVars(query);

            // Parse and sanitize 'include', for use by 'orderby' as well as 'include' below.
            if (QueryVars.ContainsKey("include") && QueryVars["include"] is List<int> includeList)
            {
                QueryVars["include"] = includeList.Distinct().ToList();
            }

            // Parse meta query.
            MetaQuery = new WP_Meta_Query();
            MetaQuery.ParseQueryVars(QueryVars);
        }

        /// <summary>
        /// Executes the query with the current variables.
        /// </summary>
        public void Query()
        {
            using (var context = new WordPressDbContext())
            {
                var usersQuery = context.Users.AsQueryable();

                // Apply filters based on query variables.
                if (QueryVars.ContainsKey("include") && QueryVars["include"] is List<int> includeList)
                {
                    usersQuery = usersQuery.Where(u => includeList.Contains(u.ID));
                }

                if (QueryVars.ContainsKey("exclude") && QueryVars["exclude"] is List<int> excludeList)
                {
                    usersQuery = usersQuery.Where(u => !excludeList.Contains(u.ID));
                }

                if (QueryVars.ContainsKey("search") && QueryVars["search"] is string search)
                {
                    var searchColumns = GetSearchColumns();
                    usersQuery = usersQuery.Where(u =>
                        searchColumns.Any(column =>
                            EF.Functions.Like((string)u.GetType().GetProperty(column).GetValue(u), $"%{search}%")
                        )
                    );
                }

                if (QueryVars.ContainsKey("meta_query") && MetaQuery.Queries.Any())
                {
                    foreach (var metaQuery in MetaQuery.Queries)
                    {
                        usersQuery = ApplyMetaQuery(usersQuery, metaQuery);
                    }
                }

                // Apply ordering.
                if (QueryVars.ContainsKey("orderby") && QueryVars["orderby"] is string orderBy)
                {
                    usersQuery = ApplyOrderBy(usersQuery, orderBy, QueryVars.ContainsKey("order") ? QueryVars["order"].ToString() : "ASC");
                }

                // Apply pagination.
                if (QueryVars.ContainsKey("number") && QueryVars["number"] is int number)
                {
                    usersQuery = usersQuery.Take(number);
                }

                if (QueryVars.ContainsKey("offset") && QueryVars["offset"] is int offset)
                {
                    usersQuery = usersQuery.Skip(offset);
                }

                // Execute the query.
                Results = usersQuery.ToList();
                TotalUsers = Results.Count;
            }
        }

        /// <summary>
        /// Fills in missing query variables with default values.
        /// </summary>
        /// <param name="args">Query variables.</param>
        /// <returns>Complete query variables with undefined ones filled in with defaults.</returns>
        private Dictionary<string, object> FillQueryVars(Dictionary<string, object> args)
        {
            var defaults = new Dictionary<string, object>
            {
                { "blog_id", 1 },
                { "role", "" },
                { "meta_key", "" },
                { "meta_value", "" },
                { "meta_compare", "=" },
                { "include", new List<int>() },
                { "exclude", new List<int>() },
                { "search", "" },
                { "orderby", "ID" },
                { "order", "ASC" },
                { "number", 0 },
                { "offset", 0 },
                { "count_total", true }
            };

            foreach (var key in defaults.Keys)
            {
                if (!args.ContainsKey(key))
                {
                    args[key] = defaults[key];
                }
            }

            return args;
        }

        /// <summary>
        /// Gets the columns to search in a user query.
        /// </summary>
        /// <returns>List of column names to be searched.</returns>
        private List<string> GetSearchColumns()
        {
            return new List<string> { "user_login", "user_email", "user_nicename", "display_name" };
        }

        /// <summary>
        /// Applies a meta query to the user query.
        /// </summary>
        /// <param name="query">The user query.</param>
        /// <param name="metaQuery">The meta query to apply.</param>
        /// <returns>The updated user query.</returns>
        private IQueryable<User> ApplyMetaQuery(IQueryable<User> query, MetaQuery metaQuery)
        {
            return query.Where(u => u.MetaData.Any(m =>
                m.Key == metaQuery.Key &&
                CompareMetaDataValue(m.Value, metaQuery.Value, metaQuery.Compare)
            ));
        }

        /// <summary>
        /// Compares meta data value with the given value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="compare">The comparison operator.</param>
        /// <returns>Whether the values match the comparison.</returns>
        private bool CompareMetaDataValue(string value1, string value2, string compare)
        {
            switch (compare)
            {
                case "=":
                    return value1 == value2;
                case "!=":
                    return value1 != value2;
                case ">":
                    return string.Compare(value1, value2) > 0;
                case "<":
                    return string.Compare(value1, value2) < 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Applies ordering to the user query.
        /// </summary>
        /// <param name="query">The user query.</param>
        /// <param name="orderBy">The field to order by.</param>
        /// <param name="order">The order direction.</param>
        /// <returns>The ordered user query.</returns>
        private IQueryable<User> ApplyOrderBy(IQueryable<User> query, string orderBy, string order)
        {
            var property = typeof(User).GetProperty(orderBy);
            if (property == null)
            {
                throw new ArgumentException($"Invalid orderby field: {orderBy}");
            }

            return order.ToUpper() == "ASC"
                ? query.OrderBy(u => property.GetValue(u))
                : query.OrderByDescending(u => property.GetValue(u));
        }
    }

    /// <summary>
    /// Represents a meta query.
    /// </summary>
    public class MetaQuery
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Compare { get; set; }
    }

    /// <summary>
    /// Represents a user meta query.
    /// </summary>
    public class WP_Meta_Query
    {
        public List<MetaQuery> Queries { get; set; } = new();

        public void ParseQueryVars(Dictionary<string, object> queryVars)
        {
            if (queryVars.ContainsKey("meta_query") && queryVars["meta_query"] is List<Dictionary<string, object>> metaQueries)
            {
                foreach (var metaQuery in metaQueries)
                {
                    Queries.Add(new MetaQuery
                    {
                        Key = metaQuery.ContainsKey("key") ? metaQuery["key"].ToString() : "",
                        Value = metaQuery.ContainsKey("value") ? metaQuery["value"].ToString() : "",
                        Compare = metaQuery.ContainsKey("compare") ? metaQuery["compare"].ToString() : "="
                    });
                }
            }
        }
    }

    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        public int ID { get; set; }
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        public string UserNicename { get; set; }
        public string DisplayName { get; set; }
        public List<MetaData> MetaData { get; set; } = new();
    }

    /// <summary>
    /// Represents user meta data.
    /// </summary>
    public class MetaData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// WordPress database context.
    /// </summary>
    public class WordPressDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}