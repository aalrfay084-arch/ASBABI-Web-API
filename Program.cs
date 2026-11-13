using LibaryManagement.Data;
using LibaryManagement.Identity;
using LibaryManagement.Services;
using LibaryManagement.Services.Implemenations;
using LibaryManagement.Services.Interfaces;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyConnection")
    ));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!))
    };
});

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4301")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "أدخل JWT Token هنا."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<PurchaseExpirationBackgroundService>();
var app = builder.Build();
// إنشاء الأدوار تلقائياً عند تشغيل المشروع

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var roleManager = serviceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = serviceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    // Create Roles
    string[] roles =
    {
        "SuperAdmin",
        "Admin",
        "User"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // SuperAdmin Settings
    var configuration = serviceProvider
        .GetRequiredService<IConfiguration>();

    var adminEmail = configuration["SuperAdmin:Email"];
    var adminPassword = configuration["SuperAdmin:Password"];
    var adminFullName = configuration["SuperAdmin:FullName"];

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword) ||
        string.IsNullOrWhiteSpace(adminFullName))
    {
        throw new Exception("SuperAdmin settings are missing.");
    }

    // Find SuperAdmin
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // Create SuperAdmin if not exists
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            FullName = adminFullName,
            UserName = adminEmail,
            Email = adminEmail
        };

        var result = await userManager.CreateAsync(
            adminUser,
            adminPassword
        );

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)
                )
            );
        }
    }

    // Assign SuperAdmin Role
    if (!await userManager.IsInRoleAsync(adminUser, "SuperAdmin"))
    {
        var roleResult = await userManager.AddToRoleAsync(
            adminUser,
            "SuperAdmin"
        );

        if (!roleResult.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    roleResult.Errors.Select(e => e.Description)
                )
            );
        }
    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularClient");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


