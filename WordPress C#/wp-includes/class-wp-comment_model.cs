public class Comment
        public virtual Post Post { get; set; }

        // Additional fields for caching and children population
        public bool PopulatedChildren { get; set; }
    }

    /// <summary>
    /// Represents a post in the system.
    /// </summary>
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostDate { get; set; }
        public DateTime PostDateGmt { get; set; }
        public string Status { get; set; } = "publish";

        // Navigation property
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }