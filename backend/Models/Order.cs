namespace backend.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }

    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}