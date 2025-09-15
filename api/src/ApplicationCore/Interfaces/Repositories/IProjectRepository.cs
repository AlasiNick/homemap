using Homemap.Domain.Core;

namespace Homemap.ApplicationCore.Interfaces.Repositories;

public interface IProjectRepository : ICrudRepository<Project>
{
    Task<IReadOnlyList<Project>> FindAllByUserIdAsync(Guid userId);
}

