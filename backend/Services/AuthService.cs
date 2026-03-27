namespace backend.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models;
using backend.Dtos;
using Microsoft.EntityFrameworkCore;
using backend.Data;

public class AuthService
{
    private readonly RetailOrderingContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(RetailOrderingContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterDTO request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Email already registered");

        // Create new user
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            PasswordHash = HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginDTO request)
    {
        // Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            throw new Exception("Invalid email or password");

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        // Generate token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var secretKey = jwtSettings["Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}