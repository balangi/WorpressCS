public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Comment> GetCommentById(int id) => await _context.Comments.FindAsync(id);

    public async Task<List<Comment>> GetCommentsByPostId(int postId) =>
        await _context.Comments.Where(c => c.PostId == postId && c.Status == "approved").ToListAsync();

    public async Task<int> AddComment(Comment comment)
    {
        if (string.IsNullOrEmpty(comment.Type)) comment.Type = "comment";

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment.Id;
    }

    public async Task<bool> UpdateComment(Comment comment)
    {
        _context.Comments.Update(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return false;

        _context.Comments.Remove(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ApproveComment(int id) => await UpdateStatus(id, "approved");
    public async Task<bool> SpamComment(int id) => await UpdateStatus(id, "spam");
    public async Task<bool> TrashComment(int id) => await UpdateStatus(id, "trash");

    private async Task<bool> UpdateStatus(int id, string status)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return false;

        comment.Status = status;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CommentCount> GetCommentCounts(int? postId = null)
    {
        var query = _context.Comments.AsQueryable();
        if (postId.HasValue) query = query.Where(c => c.PostId == postId.Value);

        return new CommentCount
        {
            Approved = await query.CountAsync(c => c.Status == "approved"),
            Pending = await query.CountAsync(c => c.Status == "pending"),
            Spam = await query.CountAsync(c => c.Status == "spam"),
            Trash = await query.CountAsync(c => c.Status == "trash"),
            Total = await query.CountAsync()
        };
    }

    public async Task<List<Comment>> SearchComments(string keyword, int? postId = null)
    {
        var query = _context.Comments.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(c => EF.Functions.Like(c.Content, $"%{keyword}%"));
        }

        if (postId.HasValue)
        {
            query = query.Where(c => c.PostId == postId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task AddCommentMeta(int commentId, string key, object value)
    {
        await _context.CommentMetas.AddAsync(new CommentMeta
        {
            CommentId = commentId,
            Key = key,
            Value = JsonConvert.SerializeObject(value)
        });
        await _context.SaveChangesAsync();
    }

    public async Task<T> GetCommentMeta<T>(int commentId, string key, bool single = true)
    {
        var meta = await _context.CommentMetas.FirstOrDefaultAsync(m => m.CommentId == commentId && m.Key == key);
        if (meta == null) return default;

        return JsonConvert.DeserializeObject<T>(meta.Value);
    }

    public async Task<bool> DeleteCommentMeta(int commentId, string key)
    {
        var metas = await _context.CommentMetas.Where(m => m.CommentId == commentId && m.Key == key).ToListAsync();
        if (metas.Count == 0) return false;

        _context.CommentMetas.RemoveRange(metas);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCommentMeta(int commentId, string key, object value)
    {
        var meta = await _context.CommentMetas.FirstOrDefaultAsync(m => m.CommentId == commentId && m.Key == key);
        if (meta == null)
        {
            await AddCommentMeta(commentId, key, value);
            return true;
        }

        meta.Value = JsonConvert.SerializeObject(value);
        _context.CommentMetas.Update(meta);
        return await _context.SaveChangesAsync() > 0;
    }
}