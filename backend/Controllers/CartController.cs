namespace backend.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly RetailOrderingContext _context;

    public CartController(RetailOrderingContext context)
    {
        _context = context;
    }

    // GET /api/cart
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if (cart == null)
            return NotFound(new { message = "Cart not found" });
        
        return Ok(cart);
    }

    // POST /api/cart/add
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        if (request.Quantity <= 0)
            return BadRequest(new { message = "Quantity must be greater than 0" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == request.ProductId);

        if (cartItem != null)
        {
            cartItem.Quantity += request.Quantity;
            _context.CartItems.Update(cartItem);
        }
        else
        {
            cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Item added to cart", cartItemId = cartItem.Id, quantity = cartItem.Quantity });
    }

    // DELETE /api/cart/remove/{cartItemId}
    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        
        if (cartItem == null)
            return NotFound(new { message = "Cart item not found" });
        
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Item removed from cart" });
    }
}

public class AddToCartRequest
{
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
}