using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string from secrets.json
var connectionString = builder.Configuration["SqlConnectionString"]
    ?? throw new InvalidOperationException("Connection string is missing");

// Add scopes, identity and authorization
builder.Services.AddScopes(connectionString);
builder.Services.AddIdentity(connectionString);
builder.Services.AddAuthorization();

// Rate limiter: Max 5 requests, per 10 seconds
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10)
            }
        ));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection(); // Redirect all http requests to https
app.UseAuthentication();
app.UseAuthorization();

//Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.TryAdd("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    await next();
});

app.UseRateLimiter();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "API is up");
app.Run();