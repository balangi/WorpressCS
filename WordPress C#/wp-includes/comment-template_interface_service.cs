public interface ICommentTemplateService
{
    string GetCommentAuthor(int commentId);
    string GetCommentAuthorLink(int commentId);
    string GetCommentAuthorIP(int commentId);
    string GetCommentContent(int commentId);
    string GetCommentDate(int commentId, string format = null, bool gmt = false);
    string GetCommentTime(int commentId, string format = null, bool gmt = false);
    string GetCommentExcerpt(int commentId, int maxLength = 50);
    string GetCommentLink(int commentId, int? cpage = null);
    string RenderCommentList(List<Comment> comments, int postId, int depth = 1);
    string RenderCommentForm(int postId, string replyTo = null);
}