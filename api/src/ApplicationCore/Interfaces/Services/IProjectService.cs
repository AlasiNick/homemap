using ErrorOr;
using Homemap.ApplicationCore.Models;

namespace Homemap.ApplicationCore.Interfaces.Services
{
    public interface IProjectService : IService<ProjectDto>
    {
        IAsyncEnumerable<ErrorOr<DeviceLogDto>> GetDeviceLogsByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProjectDto>> GetAllByUserIdAsync(Guid userId);
        Task<ProjectDto> CreateForUserAsync(Guid userId, ProjectDto dto);
    }
}
