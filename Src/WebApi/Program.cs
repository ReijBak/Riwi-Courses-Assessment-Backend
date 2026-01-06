using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Riwi.CoursesAssessment.Infrastructure;
using Riwi.CoursesAssessment.Infrastructure.Data;
using Riwi.CoursesAssessment.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure OpenAPI/Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Riwi Courses Assessment API",
        Version = "v1",
        Description = "API REST para la gestión de cursos y lecciones con autenticación JWT",
        Contact = new OpenApiContact
        {
            Name = "Riwi",
            Email = "info@riwi.io"
        }
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
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
            Array.Empty<string>()
        }
    });
});

// Add Infrastructure services (DbContext, Identity, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
    };
});

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations and seed data on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            app.Logger.LogInformation("Attempting to apply database migrations (attempt {Attempt}/{MaxRetries})...", i + 1, maxRetries);
            await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
            app.Logger.LogInformation("Database migrations and seeding completed successfully.");
            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            app.Logger.LogWarning(ex, "Database not ready, retrying in {Delay} seconds... (attempt {Attempt}/{MaxRetries})", delay.TotalSeconds, i + 1, maxRetries);
            await Task.Delay(delay);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to apply migrations after {MaxRetries} attempts.", maxRetries);
            throw;
        }
    }
}

// Configure the HTTP request pipeline

// Exception handling middleware (debe ir primero)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger in Development and Docker environments
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Riwi Courses API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Authentication & Authorization (el orden importa)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

