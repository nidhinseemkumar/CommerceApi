using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CommerceApi.Services;
using CommerceApi.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                System.Diagnostics.Debug.WriteLine("Claims: " + string.Join(", ", claims ?? Array.Empty<string>()));
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                System.Diagnostics.Debug.WriteLine("ERROR");
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                if (context.AuthenticateFailure == null && string.IsNullOrEmpty(context.Error))
                {
                    System.Diagnostics.Debug.WriteLine("MISSING");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAuthService, AuthService>();

// Product Service with Cache Decorator
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductService, CachedProductService>();

// Category Service with Cache Decorator
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ICategoryService, CachedCategoryService>();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();