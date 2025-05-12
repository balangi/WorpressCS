public interface ICommentService
{
    Task<Comment> GetCommentById(int id);
    Task<List<Comment>> GetCommentsByPostId(int postId);
    Task<int> AddComment(Comment comment);
    Task<bool> UpdateComment(Comment comment);
    Task<bool> DeleteComment(int id);
    Task<bool> ApproveComment(int id);
    Task<bool> SpamComment(int id);
    Task<bool> TrashComment(int id);
    Task<CommentCount> GetCommentCounts(int? postId = null);
    Task<List<Comment>> SearchComments(string keyword, int? postId = null);
    Task AddCommentMeta(int commentId, string key, object value);
    Task<T> GetCommentMeta<T>(int commentId, string key, bool single = true);
    Task<bool> DeleteCommentMeta(int commentId, string key);
    Task<bool> UpdateCommentMeta(int commentId, string key, object value);
}