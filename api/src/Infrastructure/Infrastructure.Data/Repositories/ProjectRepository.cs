using Homemap.ApplicationCore.Interfaces.Repositories;
using Homemap.Domain.Core;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Repositories;

internal class ProjectRepository(ApplicationDbContext context)
    : CrudRepository<Project>(context), IProjectRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IReadOnlyList<Project>> FindAllByUserIdAsync(Guid userId)
    {
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }
}

