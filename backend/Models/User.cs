namespace backend.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
