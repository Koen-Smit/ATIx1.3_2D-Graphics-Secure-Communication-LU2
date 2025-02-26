using Microsoft.AspNetCore.Authorization;

public static class Authorization
{
    public static void AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanReadEntity", policy =>
                policy.RequireClaim("entity:read", "true"));

            options.AddPolicy("CanWriteEntity", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "entity:write" && c.Value == "true") ||
                    context.User.IsInRole("Admin")));

            options.AddPolicy("CanDeleteEntity", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "entity:delete" && c.Value == "true") ||
                    context.User.IsInRole("Admin")));

            options.AddPolicy("AccessEntity", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "entity:read" && c.Value == "true") &&
                    context.User.HasClaim(c => c.Type == "entity:write" && c.Value == "true") &&
                    context.User.HasClaim(c => c.Type == "entity:delete" && c.Value == "true") &&
                    context.User.IsInRole("Admin")));
        });
    }
}
