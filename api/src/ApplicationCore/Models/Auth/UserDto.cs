using Homemap.ApplicationCore.Models.Common;

namespace Homemap.ApplicationCore.Models.Auth;

public record UserDto : AuditableEntityDto
{
    public required string Email { get; set; }
    public required string Name { get; set; }
}
