using Microsoft.AspNetCore.Identity;

public static class Identity
{
    public static void AddIdentity(this IServiceCollection services, string connectionString)
    {
        services.AddHttpContextAccessor();

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
            options.Password.RequiredLength = 10;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddDapperStores(options =>
        {
            options.ConnectionString = connectionString;
        });
    }
}
