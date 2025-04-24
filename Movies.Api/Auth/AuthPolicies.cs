namespace Movies.Api.Auth;

public static class AuthPolicies
{
    public static IServiceCollection AddAuthPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthConstants.AdminPolicy, builder =>
            {
                builder.RequireClaim(AuthConstants.AdminClaimType, AuthConstants.AdminClaimValue);
            })
            .AddPolicy(AuthConstants.TrustedUserPolicy, builder =>
            {
                builder.RequireClaim(AuthConstants.TrustedUserClaim, AuthConstants.TrustedUserClaimValue);
            });
        
        return services;
    }
}