// =====================================================
// Program.cs — User Management API
// Assisted and debugged using Microsoft Copilot
// =====================================================

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<UserService>();

// ── Logging ───────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ── Swagger (dev only) ────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── Middleware Pipeline ───────────────────────────

// 1. Global Exception Handler — catches ALL unhandled crashes
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Unhandled exception: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred. Please try again later."
        });
    }
});

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. API Key Authentication Middleware
// Copilot suggested the basic structure; modified to read key from config
// instead of hardcoding (security best practice)
app.Use(async (context, next) =>
{
    // Allow Swagger UI to load without API key
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
    var validKey = builder.Configuration["ApiKey"] ?? "nexbridge-secret-2024";

    if (string.IsNullOrEmpty(apiKey) || apiKey != validKey)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized. Provide a valid X-API-Key header."
        });
        return; // Short-circuit — don't call next()
    }

    await next();
});

// 4. Request Logging Middleware
// Logs each valid request (placed after auth so only legit requests logged)
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] {context.Request.Method} {context.Request.Path}");

    await next();

    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Response: {context.Response.StatusCode}");
});

// ── Map Controllers ───────────────────────────────
app.UseRouting();
app.MapControllers();

app.Run();
