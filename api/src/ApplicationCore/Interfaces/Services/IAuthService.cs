using Google.Apis.Auth;
using Homemap.ApplicationCore.Models.Auth;
using Homemap.Domain.Common;


namespace Homemap.ApplicationCore.Interfaces.Services;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user);
    string GenerateAccessTokenAsync(User user);
    Task<User?> ValidateAccessTokenAsync(string accessToken);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task<User> ValidateCredentialsAsync(string email, string password);
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken);
    Task<(User user, string message)> HandleGoogleUserAsync(GoogleJsonWebSignature.Payload payload);
}
