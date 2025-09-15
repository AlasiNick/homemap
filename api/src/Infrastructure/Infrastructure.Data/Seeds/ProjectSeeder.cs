using Homemap.ApplicationCore.Interfaces.Seeders;
using Homemap.Domain.Core;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Seeds
{
    public class ProjectSeeder : BaseSeeder<Project>, ISeeder
    {
        public ProjectSeeder(ApplicationDbContext context) : base(context)
        {
        }
        public async Task SeedAsync()
        {
            // If projects already exist, backfill missing UserId with admin (or first) user
            if (await _entities.AnyAsync())
            {
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@admin.com");
                var owner = admin ?? await _context.Users.FirstOrDefaultAsync();

                if (owner is not null)
                {
                    var withoutOwner = await _entities
                        .Where(p => p.UserId == Guid.Empty)
                        .ToListAsync();

                    if (withoutOwner.Count > 0)
                    {
                        foreach (var p in withoutOwner)
                            p.UserId = owner.Id;

                        await _context.SaveChangesAsync();
                    }
                }

                // We won't add demo projects if any are present already
                return;
            }

            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@admin.com");
            var firstUser = adminUser ?? await _context.Users.FirstOrDefaultAsync();

            List<Project> projects = [
                new Project()
                {
                    Name = "Dad's garage",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Home - ground floor",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },

                 new Project()
                {
                    Name = "Home - second floor",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                 new Project()
                {
                    Name = "Cottage - sauna",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Cottage - main house",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                 new Project()
                {
                    Name = "Cabin",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                 new Project()
                {
                    Name = "Office",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Guest room",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Farmhouse - kitchen",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Father's room",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
                new Project()
                {
                    Name = "Mother's room",
                    UserId = firstUser?.Id ?? Guid.Empty,
                },
            ];

            await _entities.AddRangeAsync(projects);
            await _context.SaveChangesAsync();
        }
    }
}
