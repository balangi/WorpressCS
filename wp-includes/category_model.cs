using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Category
{
    [Key]
    public int TermId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Slug { get; set; }
    
    public string Description { get; set; }
    
    public int Parent { get; set; }
    
    public int Count { get; set; }
    
    // Properties for backwards compatibility
    [NotMapped]
    public int Cat_ID => TermId;
    
    [NotMapped]
    public int Category_count => Count;
    
    [NotMapped]
    public string Category_description => Description;
    
    [NotMapped]
    public string Cat_name => Name;
    
    [NotMapped]
    public string Category_nicename => Slug;
    
    [NotMapped]
    public int Category_parent => Parent;
}