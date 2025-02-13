using LU2_WebApi.Repositorys;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is missing");
}

// Register repositories and pass the connection string to them
builder.Services.AddTransient(o => new IEnvironment2DRepository(connectionString));
builder.Services.AddTransient(o => new IObject2DRepository(connectionString));

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
