namespace Movies.Api.Auth;

public class AuthConstants
{
    public const string AdminPolicy = "Admin";
    public const string TrustedUserPolicy = "TrustedUser";
    
    public const string AdminClaimType = "isAdmin";
    public const string AdminClaimValue = "true";
    
    public const string TrustedUserClaim = "isTrustedUser";
    public const string TrustedUserClaimValue = "true";
    
    public const string UserIdClaimType = "userId";
}