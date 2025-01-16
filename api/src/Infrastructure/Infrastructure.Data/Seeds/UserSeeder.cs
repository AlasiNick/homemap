using Homemap.ApplicationCore.Interfaces.Seeders;
using Homemap.ApplicationCore.Models.Auth;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Seeds
{
    public class UserSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserSeeder(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
                return;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@admin.com",
                Name = "admin",
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "admin");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
