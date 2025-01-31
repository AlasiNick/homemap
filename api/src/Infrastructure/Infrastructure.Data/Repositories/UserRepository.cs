using Homemap.ApplicationCore.Interfaces.Repositories;
using Homemap.ApplicationCore.Models.Auth;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = context;

        /* Should we conducts checks/validations for null here? */
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email) ??
                throw new KeyNotFoundException($"User with email {email} not found.");
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id) ??
                throw new KeyNotFoundException($"User with id {id} not found.");
            return user;
        }

        public async Task<UserSession> GetSessionByRefreshTokenAsync(string refreshToken)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken) ??
                throw new KeyNotFoundException($"Session with refresh token {refreshToken} not found.");
            return session;
        }

        public async Task AddSessionAsync(UserSession session)
        {
            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByGoogleIdAsync(string googleId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email, bool throwIfNotFound = false)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (throwIfNotFound && user == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }

            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
