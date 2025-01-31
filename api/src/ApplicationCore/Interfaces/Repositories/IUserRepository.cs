using Homemap.ApplicationCore.Models.Auth;


namespace Homemap.ApplicationCore.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(Guid id);
    Task<UserSession> GetSessionByRefreshTokenAsync(string refreshToken);
    Task AddSessionAsync(UserSession session);
    Task CreateUserAsync(User user);
    Task<User> GetByGoogleIdAsync(string googleId);
    Task AddAsync(User user);
    Task UpdateUserAsync(User user);
    Task<User> GetUserByEmailAsync(string email, bool throwIfNotFound = false);
}
