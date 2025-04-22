// Models/Post.cs
public class Post
{
    public int Id { get; set; }
    public string PostName { get; set; }
    public string PostType { get; set; }
    public string PostStatus { get; set; }
    public DateTime PostDate { get; set; }
    public int PostParent { get; set; }
    public int PostAuthor { get; set; }
}

// Models/Term.cs
public class Term
{
    public int TermId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Taxonomy { get; set; }
}

// Services/IUrlService.cs
public interface IUrlService
{
    string GetPermalink(int postId);
    string GetPostCommentsFeedLink(int postId, string feed);
    string GetTermLink(int termId, string taxonomy);
    string GetAuthorPostsUrl(int authorId, string authorNicename);
    string GetDayLink(int year, int month, int day);
    string GetMonthLink(int year, int month);
    string GetYearLink(int year);
    string GetAttachmentLink(int attachmentId);
    string HomeUrl(string path = "");
    string SiteUrl(string path = "");
    string WpLoginUrl();
    string AdminUrl(string path = "");
}

// Services/IRewriteService.cs
public interface IRewriteService
{
    bool UsingPermalinks();
    bool UsingIndexPermalinks();
    string PermalinkStructure { get; }
    string Index { get; }
    string PaginationBase { get; }
    string CommentsPaginationBase { get; }
    List<string> Feeds { get; }
}