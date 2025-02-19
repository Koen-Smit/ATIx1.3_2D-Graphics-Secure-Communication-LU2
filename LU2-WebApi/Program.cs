using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["SqlConnectionString"]
    ?? throw new InvalidOperationException("Connection string is missing");

builder.Services.AddSingleton(connectionString!);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddDapperStores(options =>
    {
        options.ConnectionString = connectionString;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadEntity", policy =>
        policy.RequireClaim("entity:read", "true")); // Alleen gebruikers met deze claim mogen lezen

    options.AddPolicy("CanWriteEntity", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "entity:write" && c.Value == "true") ||
            context.User.IsInRole("Admin"))); // Admin mag altijd schrijven

    options.AddPolicy("CanDeleteEntity", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "entity:delete" && c.Value == "true") ||
            context.User.IsInRole("Admin"))); // Admin mag altijd verwijderen

    options.AddPolicy("Level8WizardOnly", policy =>
    {
        policy.RequireClaim("wizard");
        policy.RequireClaim("wizardLevel", "8");
    });
});

var app = builder.Build();

// Force HTTPS in production and redirect HTTP to HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP naar HTTPS
app.UseAuthorization();

app.MapGroup(prefix: "/account")
    .MapIdentityApi<IdentityUser>();

app.MapPost(pattern: "/account/logout",
    async (SignInManager<IdentityUser> signInManager,
    [FromBody] object empty) => {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
    .RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers()
    .RequireAuthorization();

app.MapGet("/debug/claims", (HttpContext httpContext) =>
{
    var claims = httpContext.User.Claims
        .Select(c => new { c.Type, c.Value })
        .ToList();

    return claims.Any() ? Results.Json(claims) : Results.Unauthorized();
}).RequireAuthorization();

app.MapGet("/", () => "API is up");

app.Run();
