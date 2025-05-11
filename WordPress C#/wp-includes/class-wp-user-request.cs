using System;
using System.Collections.Generic;
using System.Text.Json;

namespace WordPress.Users
{
    /// <summary>
    /// Represents user request data loaded from a WP_Post object.
    /// </summary>
    public class WP_User_Request
    {
        /// <summary>
        /// Request ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// User email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Action name.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Current status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Timestamp this request was created.
        /// </summary>
        public DateTime? CreatedTimestamp { get; set; }

        /// <summary>
        /// Timestamp this request was last modified.
        /// </summary>
        public DateTime? ModifiedTimestamp { get; set; }

        /// <summary>
        /// Timestamp this request was confirmed.
        /// </summary>
        public DateTime? ConfirmedTimestamp { get; set; }

        /// <summary>
        /// Timestamp this request was completed.
        /// </summary>
        public DateTime? CompletedTimestamp { get; set; }

        /// <summary>
        /// Misc data assigned to this request.
        /// </summary>
        public Dictionary<string, object> RequestData { get; set; } = new();

        /// <summary>
        /// Key used to confirm this request.
        /// </summary>
        public string ConfirmKey { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="post">Post object.</param>
        public WP_User_Request(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post object cannot be null.");
            }

            ID = post.ID;
            UserID = post.AuthorID;
            Email = post.Title;
            ActionName = post.Slug;
            Status = post.Status;
            CreatedTimestamp = post.CreatedDate.ToUniversalTime();
            ModifiedTimestamp = post.ModifiedDate.ToUniversalTime();
            ConfirmedTimestamp = GetMetaValueAsDateTime(post.MetaData, "_wp_user_request_confirmed_timestamp");
            CompletedTimestamp = GetMetaValueAsDateTime(post.MetaData, "_wp_user_request_completed_timestamp");
            RequestData = ParseRequestData(post.Content);
            ConfirmKey = post.Password;
        }

        /// <summary>
        /// Parses the request data from JSON.
        /// </summary>
        /// <param name="content">JSON content.</param>
        /// <returns>Parsed request data as a dictionary.</returns>
        private Dictionary<string, object> ParseRequestData(string content)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(content) ?? new();
            }
            catch
            {
                return new();
            }
        }

        /// <summary>
        /// Retrieves a meta value as a DateTime.
        /// </summary>
        /// <param name="metaData">List of meta data.</param>
        /// <param name="key">Meta key.</param>
        /// <returns>DateTime value or null.</returns>
        private DateTime? GetMetaValueAsDateTime(List<MetaData> metaData, string key)
        {
            var meta = metaData.FirstOrDefault(m => m.Key == key);
            if (meta != null && long.TryParse(meta.Value, out var timestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            }

            return null;
        }
    }

    /// <summary>
    /// Represents a post.
    /// </summary>
    public class Post
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Content { get; set; }
        public string Password { get; set; }
        public List<MetaData> MetaData { get; set; } = new();
    }

    /// <summary>
    /// Represents meta data for a post.
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
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}