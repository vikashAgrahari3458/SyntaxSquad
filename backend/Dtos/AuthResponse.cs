namespace backend.Dtos;

public class AuthResponse
{
    public required string Token { get; set; }
    public required UserDto User { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}