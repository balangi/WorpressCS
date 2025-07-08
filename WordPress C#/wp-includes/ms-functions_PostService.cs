public class PostService
{
    private readonly ApplicationDbContext _context;

    public PostService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void UpdatePostCount(int siteId)
    {
        var postCount = _context.Posts.Count(p => p.SiteId == siteId && p.Status == "publish");
        var site = _context.Sites.FirstOrDefault(s => s.Id == siteId);
        if (site != null)
        {
            site.PostCount = postCount;
            _context.SaveChanges();
        }
    }
}