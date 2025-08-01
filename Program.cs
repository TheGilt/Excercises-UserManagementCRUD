using Microsoft.OpenApi;
using Swashbuckle.AspNetCore;
using System.ComponentModel.DataAnnotations; // add for validation attributes

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Swagger options here configured by copilot to allow testing passed auth
builder.Services.AddSwaggerGen(options =>
{
    // Add a custom header parameter to all endpoints
    options.AddSecurityDefinition("X-Api-Token", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-Api-Token",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Custom token header"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "X-Api-Token"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddControllers();

var app = builder.Build();

// Exception handling middleware (should be first)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected server error: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected server error occurred.");
    }
});

// Simulated Authentication middleware (skip Swagger/OpenAPI endpoints)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path.StartsWith("/swagger") || path.StartsWith("/openapi"))
    {
        await next();
        return;
    }
    var token = context.Request.Headers["X-Api-Token"].FirstOrDefault();
    if (token != "secret_password")
    {
        Console.WriteLine($"Security event: unauthenticated access attempt - {context.Request.Path}");
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("User cannot be authenticated.");
        return;
    }
    await next();
});

// Logging middleware (copilot moved before controllers)
app.Use(async (context, next) =>
{
    await next();
    Console.WriteLine($"Request: {context.Request.Method}\n\t{context.Request.Path}\n\t{context.Response.StatusCode}");
});

app.UseSwagger();
app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

// Created with copilot, and then copilot debugged to include required tags
public class User
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Id must be a non-negative integer.")]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}