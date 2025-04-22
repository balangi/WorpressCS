public class Post
{
    [Key]
    public int ID { get; set; }
    
    public ICollection<PostCategory> PostCategories { get; set; }
}

public class PostCategory
{
    public int PostId { get; set; }
    public Post Post { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}