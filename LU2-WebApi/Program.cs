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


var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "API is up");
app.Run();