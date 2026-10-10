using ClothingShop.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ClothingShopDbContext>();

var key = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));
var validation = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "JwtDemo",

        ValidateAudience = true,
        ValidAudience = "JwtDemoApi",

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = "name",
        RoleClaimType = "role"
    };

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = validation;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var hasher = new PasswordHasher<User>();

app.MapPost("/api/auth/register", async (ClothingShopDbContext ctx, RegisterRequest req) =>
{
    if (ctx.Users.Any(u => u.Login == req.Login))
        return Results.BadRequest(new { message = "Логин уже занят" });

    var user = new User
    {
        Login = req.Login,
        Phone = req.Phone,
        Address = req.Address,
        Role = "User"
    };
    user.PasswordHash = hasher.HashPassword(user, req.Password);

    ctx.Users.Add(user);
    await ctx.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Login });
}).AllowAnonymous();

app.MapPost("/api/auth/login", (ClothingShopDbContext ctx, LoginRequest req) =>
    {
        var user = ctx.Users.FirstOrDefault(u => u.Login == req.Login);
        if (user == null || string.IsNullOrWhiteSpace(req.Password))
            return Results.Challenge();

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result == PasswordVerificationResult.Failed)
            return Results.Challenge();

        return Results.Ok(new
        {
            access_token = CreateToken(user),
            token_type = "Bearer"
        });
    }).AllowAnonymous();

app.MapGet("/api/me", (ClaimsPrincipal principal) =>
    Results.Ok(new
    {
        login = principal.Identity?.Name,
        role = principal.FindFirst("role")?.Value
    })).RequireAuthorization();

app.MapGet("/api/clothes", (ClothingShopDbContext ctx) =>
    Results.Json(ctx.Clothes.ToList()));

app.MapGet("/api/clothes/search", (ClothingShopDbContext ctx, string q) =>
    Results.Json(ctx.Clothes.Where(c => c.Name.Contains(q)).ToList()));

app.MapPost("/api/orders", async (ClothingShopDbContext ctx, ClaimsPrincipal principal, OrderRequest req) =>
{
    var userId = int.Parse(principal.FindFirst("sub")!.Value);

    var order = new Order
    {
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        Status = "New"
    };
    ctx.Orders.Add(order);
    await ctx.SaveChangesAsync();

    foreach (var item in req.Items)
    {
        ctx.OrderItems.Add(new OrderItem
        {
            OrderId = order.Id,
            SizeId = item.SizeId,
            Quantity = item.Quantity
        });
    }
    await ctx.SaveChangesAsync();

    return Results.Created($"/api/orders/{order.Id}", new { order.Id, order.Status });
}).RequireAuthorization();

app.MapGet("/api/orders/by-status/{status}", (ClothingShopDbContext ctx, string status) =>
Results.Json(ctx.Orders.Where(o => o.Status == status).ToList()))
.RequireAuthorization();

app.MapGet("/api/orders/by-user/{userId}", (ClothingShopDbContext ctx, int userId) =>
Results.Json(ctx.Orders.Where(o => o.UserId == userId).ToList()))
.RequireAuthorization();

app.MapPatch("/api/orders/{id}/status",
async (ClothingShopDbContext ctx, int id, StatusRequest req) =>
{
    var order = await ctx.Orders.FindAsync(id);
    if (order == null) return Results.NotFound(new { message = "Заказ не найден" });

    order.Status = req.Status;
    await ctx.SaveChangesAsync();
    return Results.Ok(order);
})
.RequireAuthorization(p => p.RequireRole("Manager"));

app.MapPatch("/api/sizes/{id}/quantity",
async (ClothingShopDbContext ctx, int id, QuantityRequest req) =>
{
    var size = await ctx.Sizes.FindAsync(id);
    if (size == null) return Results.NotFound(new { message = "Размер не найден" });

    size.Quantity = req.Quantity;
    await ctx.SaveChangesAsync();
    return Results.Ok(size);
})
.RequireAuthorization(p => p.RequireRole("Manager"));

app.MapPatch("/api/clothes/{id}/hide",
async (ClothingShopDbContext ctx, int id) =>
{
    var cloth = await ctx.Clothes.FindAsync(id);
    if (cloth == null) return Results.NotFound(new { message = "Товар не найден" });

    cloth.IsAvailable = false;
    await ctx.SaveChangesAsync();
    return Results.Ok(cloth);
})
.RequireAuthorization(p => p.RequireRole("Manager"));

app.MapPatch("/api/clothes/{id}/show",
async (ClothingShopDbContext ctx, int id) =>
{
    var cloth = await ctx.Clothes.FindAsync(id);
    if (cloth == null) return Results.NotFound(new { message = "Товар не найден" });

    cloth.IsAvailable = true;
    await ctx.SaveChangesAsync();
    return Results.Ok(cloth);
})
.RequireAuthorization(p => p.RequireRole("Manager"));


app.Run();

string CreateToken(User user)
{
    var claims = new[]
    {
        new Claim("sub", user.Id.ToString()),
        new Claim("name", user.Login),
        new Claim("role", user.Role)
    };

    var token = new JwtSecurityToken(
        issuer: "JwtDemo",
        audience: "JwtDemoApi",
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(60),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(token);
}

public record LoginRequest(string Login, string Password);
public record RegisterRequest(string Login, string Password, string Phone, string Address);

public record OrderRequest(List<OrderItemRequest> Items);
public record OrderItemRequest(int SizeId, int Quantity);
public record StatusRequest(string Status);
public record QuantityRequest(int Quantity);