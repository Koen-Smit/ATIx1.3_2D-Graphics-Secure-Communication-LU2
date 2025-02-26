using Microsoft.AspNetCore.Identity;

public static class Scopes
{
    public static void AddScopes(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IEnvironmentRepository>(sp =>
            new EnvironmentRepository(connectionString));

        services.AddScoped<IEntityRepository>(sp =>
            new EntityRepository(connectionString));

        services.AddScoped<IAccountRepository>(sp =>
        {
            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var signInManager = sp.GetRequiredService<SignInManager<AppUser>>();
            return new AccountRepository(userManager, signInManager, connectionString);
        });

        services.AddScoped<IAuthenticationService, AspNetIdentityAuthenticationService>();
    }
}
