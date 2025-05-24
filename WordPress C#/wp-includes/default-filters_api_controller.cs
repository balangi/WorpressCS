[ApiController]
[Route("wp-json/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _context.Posts.ToListAsync();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }
}