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


            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "nikita.matrossov@gmail.com",
                Name = "Nikita User",
                CreatedAt = DateTime.UtcNow
            };

            user1.PasswordHash = _passwordHasher.HashPassword(user, "test1212");

            await _context.Users.AddAsync(user);
            await _context.Users.AddAsync(user1);
            await _context.SaveChangesAsync();
        }
    }
}
