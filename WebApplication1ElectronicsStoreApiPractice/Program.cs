using System.Security.Claims;
using ElectronicsStoreApiPractice.Data;
using ElectronicsStoreApiPractice.Models;
using ElectronicsStoreApiPractice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Строка подключения DefaultConnection не найдена.");

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<
    IPasswordHasher<AppUser>,
    PasswordHasher<AppUser>>();

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Настройка Jwt:Issuer не найдена.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Настройка Jwt:Audience не найдена.");

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Настройка Jwt:Key не найдена.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(jwtKey)),

                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.FromSeconds(30)
            };     
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
