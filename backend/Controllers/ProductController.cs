using backend.Data;
using backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly RetailOrderingContext _context;

    public ProductController(RetailOrderingContext context)
    {
        _context = context;
    }
    
    private ProductDTO MapToDTO(backend.Models.Product product)
    {
        return new ProductDTO
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            Category = product.Category != null ? new CategoryDTO
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                Description = product.Category.Description,
                BrandId = product.Category.BrandId,
                Brand = product.Category.Brand != null ? new BrandDTO
                {
                    Id = product.Category.Brand.Id,
                    Name = product.Category.Brand.Name,
                    Description = product.Category.Brand.Description,
                    LogoUrl = product.Category.Brand.LogoUrl
                } : null
            } : null
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products= await _context.Products
        .Include(p=>p.Category)
        .ThenInclude(c => c.Brand)
        .ToListAsync();

        var productDTOs = products.Select(MapToDTO).ToList();
        return Ok(productDTOs);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products
        .Include(p => p.Category)
        .ThenInclude(c => c.Brand)
        .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound(new{message="Product not found"});
        }

        var productDTO = MapToDTO(product);
        return Ok(productDTO);
    }
    
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ThenInclude(c => c.Brand)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
        
        var productDTOs = products.Select(MapToDTO).ToList();
        return Ok(productDTOs);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateProductStock(int id, [FromBody] UpdateStockRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        product.StockQuantity = request.StockQuantity;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Stock updated successfully", product });
    }

    public class UpdateStockRequest
    {
        public int StockQuantity { get; set; }
    }
}