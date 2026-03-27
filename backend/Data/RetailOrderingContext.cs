namespace backend.Data;

using backend.Models;
using Microsoft.EntityFrameworkCore;

public class RetailOrderingContext : DbContext
{
    public RetailOrderingContext(DbContextOptions<RetailOrderingContext>options):base(options){}
    public DbSet<User> Users { get; set; }
    public DbSet<Brand> Brands { get; set; }
public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User - Email unique constraint
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}