public class CommentTemplateService : ICommentTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IHtmlHelper _htmlHelper;

    public CommentTemplateService(ApplicationDbContext context, IHtmlHelper htmlHelper)
    {
        _context = context;
        _htmlHelper = htmlHelper;
    }

    public async Task<string> GetCommentAuthorAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return "Anonymous";

        return comment.Author ?? "Anonymous";
    }

    public async Task<string> GetCommentAuthorLinkAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (string.IsNullOrWhiteSpace(comment?.AuthorUrl)) return comment?.Author ?? "Anonymous";

        return $"<a href='{comment.AuthorUrl}' rel='external nofollow ugc'>{comment.Author}</a>";
    }

    public async Task<string> GetCommentAuthorIPAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        return comment?.AuthorIp ?? "";
    }

    public async Task<string> GetCommentContentAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        return comment?.Content ?? "";
    }

    public async Task<string> GetCommentDateAsync(int commentId, string format = null, bool gmt = false)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return "";

        var date = gmt ? comment.DateGmt : comment.Date;
        return date.ToString(format ?? "yyyy-MM-dd");
    }

    public async Task<string> GetCommentTimeAsync(int commentId, string format = null, bool gmt = false)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return "";

        var time = gmt ? comment.DateGmt : comment.Date;
        return time.ToString(format ?? "HH:mm:ss");
    }

    public async Task<string> GetCommentExcerptAsync(int commentId, int maxLength = 50)
    {
        var content = await GetCommentContentAsync(commentId);
        if (content.Length <= maxLength) return content;

        return content.Substring(0, maxLength) + "&#8230;";
    }

    public async Task<string> GetCommentLinkAsync(int commentId, int? cpage = null)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return "#";

        var link = $"/post/{comment.PostId}#comment-{comment.Id}";
        if (cpage.HasValue && cpage > 1)
        {
            link = $"{link}?cpage={cpage}";
        }

        return link;
    }

    public async Task<string> RenderCommentListAsync(List<Comment> comments, int postId, int depth = 1)
    {
        var output = new StringBuilder();

        output.AppendLine("<ul>");

        foreach (var comment in comments.Where(c => c.ParentId == null))
        {
            output.AppendLine("<li>");
            output.AppendLine(await RenderSingleCommentAsync(comment, postId, depth));
            var children = comments.Where(c => c.ParentId == comment.Id).ToList();
            if (children.Count > 0 && depth < 5)
            {
                output.AppendLine(await RenderCommentListAsync(children, postId, depth + 1));
            }
            output.AppendLine("</li>");
        }

        output.AppendLine("</ul>");
        return output.ToString();
    }

    private async Task<string> RenderSingleCommentAsync(Comment comment, int postId, int depth)
    {
        var output = new StringBuilder();

        output.AppendLine($"<div class='comment depth-{depth}'>");
        output.AppendLine($"<h4>{await GetCommentAuthorLinkAsync(comment.Id)}</h4>");
        output.AppendLine($"<p>{await GetCommentContentAsync(comment.Id)}</p>");
        output.AppendLine($"<small>Posted on {await GetCommentDateAsync(comment.Id)} at {await GetCommentTimeAsync(comment.Id)}</small>");
        output.AppendLine("</div>");

        return output.ToString();
    }

    public string RenderCommentForm(int postId, string replyTo = null)
    {
        var output = new StringBuilder();

        output.AppendLine($"<form action='/comment/post' method='post' id='commentform'>");
        output.AppendLine($"<input type='hidden' name='comment_post_ID' value='{postId}' />");
        if (!string.IsNullOrEmpty(replyTo))
        {
            output.AppendLine($"<input type='hidden' name='replytocom' value='{replyTo}' />");
        }

        output.AppendLine("<p class='comment-form-author'>");
        output.AppendLine("<label for='author'>Name</label>");
        output.AppendLine("<input type='text' name='author' id='author' required /></p>");

        output.AppendLine("<p class='comment-form-email'>");
        output.AppendLine("<label for='email'>Email</label>");
        output.AppendLine("<input type='email' name='email' id='email' required /></p>");

        output.AppendLine("<p class='comment-form-comment'>");
        output.AppendLine("<label for='comment'>Comment</label>");
        output.AppendLine("<textarea name='comment' id='comment' cols='45' rows='8' required></textarea></p>");

        output.AppendLine("<p class='form-submit'>");
        output.AppendLine("<input type='submit' value='Post Comment' /></p>");
        output.AppendLine("</form>");

        return output.ToString();
    }
}