using Homemap.ApplicationCore.Models.Auth;


namespace Homemap.ApplicationCore.Interfaces.Services;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user);
    string GenerateAccessTokenAsync(User user);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task<User> ValidateCredentialsAsync(string email, string password);
}
