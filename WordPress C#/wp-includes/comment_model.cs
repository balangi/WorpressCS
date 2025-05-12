public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string AuthorEmail { get; set; }
    public string AuthorUrl { get; set; }
    public string AuthorIp { get; set; }
    public string UserAgent { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime DateGmt { get; set; } = DateTime.UtcNow;
    public string ContentFiltered { get; set; }
    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public int? ParentId { get; set; }
    public virtual Comment Parent { get; set; }

    public string Type { get; set; } = "comment";
    public string Status { get; set; } = "approved"; // approved, pending, spam, trash
    public int Karma { get; set; } = 0;

    public int? UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public ICollection<CommentMeta> Metas { get; set; } = new List<CommentMeta>();
}

public class CommentMeta
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public virtual Comment Comment { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}

public class CommentCount
{
    public int Approved { get; set; }
    public int Pending { get; set; }
    public int Spam { get; set; }
    public int Trash { get; set; }
    public int Total { get; set; }
}