using backend.Data;
using backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly RetailOrderingContext _context;

    public CategoryController(RetailOrderingContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Include(c => c.Brand)
            .ToListAsync();

        var categoryDTOs = categories.Select(c => new CategoryDTO
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            BrandId = c.BrandId,
            Brand = c.Brand != null ? new BrandDTO
            {
                Id = c.Brand.Id,
                Name = c.Brand.Name,
                Description = c.Brand.Description,
                LogoUrl = c.Brand.LogoUrl
            } : null
        }).ToList();

        return Ok(categoryDTOs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Brand)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            BrandId = category.BrandId,
            Brand = category.Brand != null ? new BrandDTO
            {
                Id = category.Brand.Id,
                Name = category.Brand.Name,
                Description = category.Brand.Description,
                LogoUrl = category.Brand.LogoUrl
            } : null
        };

        return Ok(categoryDTO);
    }
}
