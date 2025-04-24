namespace Movies.Api.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.FindFirst(AuthConstants.UserIdClaimType)?.Value;

        if (!Guid.TryParse(userId, out var parsedId))
            return null;
        
        return parsedId;
    }
}