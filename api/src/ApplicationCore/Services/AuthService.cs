using Homemap.ApplicationCore.Interfaces.Services;
using Homemap.ApplicationCore.Models.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Homemap.ApplicationCore.Interfaces.Repositories;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Homemap.ApplicationCore.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly GoogleAuthSettings _googleSettings;

    public AuthService(
        IConfiguration configuration,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IOptions<GoogleAuthSettings> googleSettings)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _googleSettings = googleSettings.Value;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateAccessTokenAsync(user);
        var refreshToken = GenerateRefreshToken();

        var session = new UserSession
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _userRepository.AddSessionAsync(session);

        return (accessToken, refreshToken);
    }

    public string GenerateAccessTokenAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> ValidateAccessTokenAsync(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(accessToken, validationParams, out _);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return null;

            return await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
        }
        catch
        {
            return null;
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        var session = await _userRepository.GetSessionByRefreshTokenAsync(refreshToken);
        if (session == null || session.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }
        return true;
    }

    public async Task<User> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { _googleSettings.ClientId }
        };

        try
        {
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }

    public async Task<(User user, string message)> HandleGoogleUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await _userRepository.GetByGoogleIdAsync(payload.Subject);

        if (user == null)
        {
            // If not found by Google ID, try to find by email
            user = await _userRepository.GetUserByEmailAsync(payload.Email);

            if (user != null)
            {
                // User exists with email but no Google ID - link accounts
                user.GoogleId = payload.Subject;
                await _userRepository.UpdateUserAsync(user);
                return (user, "Existing account linked with Google");
            }

            // No existing user - create new
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name ?? payload.Email.Split('@')[0],
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            return (user, "New account created with Google");
        }

        return (user, "Existing Google account logged in");
    }
}
