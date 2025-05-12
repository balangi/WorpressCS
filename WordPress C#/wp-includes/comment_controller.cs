[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService) => _commentService = commentService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComment(int id)
    {
        var comment = await _commentService.GetCommentById(id);
        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _commentService.GetCommentsByPostId(postId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] Comment comment)
    {
        var id = await _commentService.AddComment(comment);
        return CreatedAtAction(nameof(GetComment), new { id }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] Comment comment)
    {
        if (id != comment.Id) return BadRequest();
        var result = await _commentService.UpdateComment(comment);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.DeleteComment(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id) => Ok(await _commentService.ApproveComment(id));

    [HttpPost("{id}/spam")]
    public async Task<IActionResult> Spam(int id) => Ok(await _commentService.SpamComment(id));

    [HttpPost("{id}/trash")]
    public async Task<IActionResult> Trash(int id) => Ok(await _commentService.TrashComment(id));

    [HttpGet("counts")]
    public async Task<IActionResult> GetCounts([FromQuery] int? postId = null)
    {
        var counts = await _commentService.GetCommentCounts(postId);
        return Ok(counts);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int? postId = null)
    {
        var results = await _commentService.SearchComments(keyword, postId);
        return Ok(results);
    }
}