namespace backend.Dtos;

public class ProductDTO
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public CategoryDTO? Category { get; set; }
}

public class CategoryDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
}

public class BrandDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}
