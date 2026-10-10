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
