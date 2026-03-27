using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Retail Ordering API",
        Version = "v1",
        Description = "API for retail ordering system"
    });
    
    // Add JWT Bearer definition for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RetailOrderingContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT");
    var secretKey = jwtSettings["Key"];
    if (!string.IsNullOrEmpty(secretKey))
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
});


builder.Services.AddScoped<AuthService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RetailOrderingContext>();
    context.Database.Migrate();
    SeedDatabase(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Retail Ordering API v1");
        options.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseCors("AllowAll");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void SeedDatabase(RetailOrderingContext context)
{
    // Always refresh seed data for demonstration purposes
    // Clear existing data
    var existingCategories = context.Categories.ToList();
    var existingProducts = context.Products.ToList();
    
    if (existingProducts.Any() || existingCategories.Any())
    {
        context.Products.RemoveRange(existingProducts);
        context.Categories.RemoveRange(existingCategories);
        context.SaveChanges();
    }

    // Create brands first
    var brands = new List<backend.Models.Brand>
    {
        new backend.Models.Brand { Name = "Pizza Palace", Description = "Premium Italian Pizzeria" },
        new backend.Models.Brand { Name = "Beverage Co", Description = "Refreshing Drinks" },
        new backend.Models.Brand { Name = "Appetizer House", Description = "Delicious Appetizers" }
    };

    context.Brands.AddRange(brands);
    context.SaveChanges();

    // Create categories with brand references
    var categories = new List<backend.Models.Category>
    {
        new backend.Models.Category { Name = "pizzas", Description = "Delicious Italian Pizzas", BrandId = brands[0].Id },
        new backend.Models.Category { Name = "drinks", Description = "Beverages and Drinks", BrandId = brands[1].Id },
        new backend.Models.Category { Name = "appetizers", Description = "Starters and Appetizers", BrandId = brands[2].Id }
    };

    context.Categories.AddRange(categories);
    context.SaveChanges();

    // Colored SVG placeholders for different product types
    const string pizzaImage = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iI0QyNDQxNiIvPjwvc3ZnPg==";
    const string drinkImage = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iIzMzMzExMSIvPjwvc3ZnPg==";
    const string appetizerImage = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iI0YwQUYwMCIvPjwvc3ZnPg==";

    // Create sample products with colored placeholder images and stock quantity of 3
    var products = new List<backend.Models.Product>
    {
        new backend.Models.Product
        {
            Name = "Margherita Pizza",
            Description = "Classic cheese and tomato pizza with fresh basil",
            Price = 299,
            StockQuantity = 3,
            CategoryId = categories[0].Id,
            ImageUrl = pizzaImage,
            IsActive = true
        },
        new backend.Models.Product
        {
            Name = "Pepperoni Pizza",
            Description = "Loaded with pepperoni and mozzarella cheese",
            Price = 349,
            StockQuantity = 3,
            CategoryId = categories[0].Id,
            ImageUrl = pizzaImage,
            IsActive = true
        },
        new backend.Models.Product
        {
            Name = "Coca Cola",
            Description = "Refreshing cola beverage",
            Price = 99,
            StockQuantity = 3,
            CategoryId = categories[1].Id,
            ImageUrl = drinkImage,
            IsActive = true
        },
        new backend.Models.Product
        {
            Name = "Garlic Bread",
            Description = "Crispy bread with garlic butter",
            Price = 149,
            StockQuantity = 3,
            CategoryId = categories[2].Id,
            ImageUrl = appetizerImage,
            IsActive = true
        }
    };

    context.Products.AddRange(products);
    context.SaveChanges();
}

