using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillForge.Api.Configuration;
using SkillForge.Api.Data;
using SkillForge.Api.HealthChecks;
using SkillForge.Api.Services;
using SkillForge.Api.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<ExchangeBackgroundServiceHealthCheck>("exchange_background_service");

// Configure request size limits for file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB limit
});
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support and file upload handling
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillForge API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // Configure file upload support
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// Configure Entity Framework with connection string from environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Please set the ConnectionStrings__DefaultConnection environment variable.");
}

// Use SQLite for production deployment (Azure SQL auth issues)
if (connectionString.Contains("Data Source="))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Configure JWT Authentication with secret from environment variables
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JWT secret key not found. Please set the JwtSettings__SecretKey environment variable.");
}

var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    
    // Enable JWT authentication for SignalR
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Configure CORS for SignalR support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins(
                "http://localhost:3000",      // Local development
                "http://frontend:3000",       // Docker container
                "http://127.0.0.1:3000",      // Alternative localhost
                "http://skillforge-frontend.australiaeast.azurecontainer.io"  // Azure production
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(origin =>
            {
                // Allow any localhost, frontend container, or Azure origins
                return origin?.StartsWith("http://localhost") == true ||
                       origin?.StartsWith("http://127.0.0.1") == true ||
                       origin?.StartsWith("http://frontend") == true ||
                       origin?.StartsWith("http://skillforge-frontend.australiaeast.azurecontainer.io") == true;
            })); // Dynamic origin validation for SignalR
});

// Configure SignalR with security settings
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Configure SignalR logging to exclude sensitive data
builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Warning);

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

// Register singleton services for real-time features
builder.Services.AddSingleton<IUserPresenceService, UserPresenceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure background service options
builder.Services.Configure<ExchangeBackgroundServiceOptions>(
    builder.Configuration.GetSection(ExchangeBackgroundServiceOptions.SectionName));

// Register background services
builder.Services.AddHostedService<ExchangeBackgroundService>();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

// Ensure uploads directory exists and configure static file serving
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");
app.MapHealthChecks("/health");

// Apply migrations on startup for all environments
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

try 
{
    Console.WriteLine("Starting database migrations...");
    dbContext.Database.Migrate();
    Console.WriteLine("Database migrations completed successfully.");
    
    // Seed the database
    DbInitializer.Initialize(dbContext);
    Console.WriteLine("Database seeding completed.");
}
catch (Exception ex)
{
    // Log but don't crash the app - important for production resilience
    Console.WriteLine($"Database migration failed: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    // Continue running - health endpoint will still work
}

app.Run();