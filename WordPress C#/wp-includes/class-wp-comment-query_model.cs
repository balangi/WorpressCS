// <summary>
    /// Represents a query for retrieving comments.
    /// </summary>
    public class CommentQueryParameters
    {
        // Query parameters
        public int? PostId { get; set; }
        public string AuthorEmail { get; set; }
        public string Status { get; set; } = "all";
        public string OrderBy { get; set; } = "comment_date_gmt";
        public string Order { get; set; } = "DESC";
        public int? ParentId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeChildren { get; set; } = true;
    }