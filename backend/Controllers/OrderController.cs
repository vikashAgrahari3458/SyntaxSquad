namespace backend.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly RetailOrderingContext _context;

    public OrderController(RetailOrderingContext context)
    {
        _context = context;
    }

    // GET /api/order
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        
        return Ok(orders);
    }

    // GET /api/order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        
        if (order == null)
            return NotFound(new { message = "Order not found" });
        
        return Ok(order);
    }

    // POST /api/order/checkout
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        Console.WriteLine($"[CHECKOUT] Received request with {request?.Items?.Count ?? 0} items");
        if (request?.Items != null)
        {
            foreach (var item in request.Items)
            {
                Console.WriteLine($"[CHECKOUT] Item: ProductId={item.ProductId}, Quantity={item.Quantity}");
            }
        }
        
        // Try to get items from request body first, then fall back to cart in database
        List<(int ProductId, int Quantity)> itemsToOrder = new();
        
        if (request?.Items != null && request.Items.Any())
        {
            // Use items from request body
            Console.WriteLine("[CHECKOUT] Using items from request body");
            itemsToOrder = request.Items
                .Select(i => (i.ProductId, i.Quantity))
                .ToList();
        }
        else
        {
            // Fall back to reading from database cart
            Console.WriteLine("[CHECKOUT] Falling back to database cart");
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null || !cart.Items.Any())
            {
                Console.WriteLine("[CHECKOUT] Cart is empty - returning error");
                return BadRequest(new { message = "Cart is empty" });
            }
            
            itemsToOrder = cart.Items
                .Select(ci => (ci.ProductId, ci.Quantity))
                .ToList();
        }

        if (!itemsToOrder.Any())
        {
            Console.WriteLine("[CHECKOUT] No items to order - returning error");
            return BadRequest(new { message = "Cart is empty" });
        }

        // Load all products and check stock
        var productIds = itemsToOrder.Select(io => io.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToListAsync();

        foreach (var item in itemsToOrder)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product == null)
                return BadRequest(new { message = $"Product {item.ProductId} not found" });
            
            if (product.StockQuantity < item.Quantity)
                return BadRequest(new { message = $"Insufficient stock for {product.Name}" });
        }

        // Create order
        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            DeliveryAddress = request.DeliveryAddress,
            SpecialInstructions = request.SpecialInstructions,
            TotalAmount = 0
        };

        // Add order items and reduce stock
        decimal totalAmount = 0;
        foreach (var (productId, quantity) in itemsToOrder)
        {
            var product = products.First(p => p.ProductId == productId);
            
            order.Items.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                PriceAtPurchase = product.Price
            });
            
            totalAmount += product.Price * quantity;
            
            // Reduce product stock
            product.StockQuantity -= quantity;
            _context.Products.Update(product);
        }
        
        order.TotalAmount = totalAmount;

        _context.Orders.Add(order);
        
        // Clear cart items if they exist in database
        var cartToClean = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cartToClean != null)
        {
            _context.CartItems.RemoveRange(cartToClean.Items);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Order placed successfully", orderId = order.Id, totalAmount = order.TotalAmount });
    }
}

public class CheckoutRequest
{
    [JsonPropertyName("deliveryAddress")]
    public required string DeliveryAddress { get; set; }
    
    [JsonPropertyName("specialInstructions")]
    public string? SpecialInstructions { get; set; }
    
    [JsonPropertyName("items")]
    public List<CheckoutItem>? Items { get; set; }
}

public class CheckoutItem
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}