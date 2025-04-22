using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Url), IsUnique = true)]
public class Feed
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Url { get; set; }
    
    [MaxLength(200)]
    public string Title { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    public DateTime LastUpdated { get; set; }
}