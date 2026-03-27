namespace backend.Models;

public class Brand
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}