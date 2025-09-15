using System.ComponentModel.DataAnnotations;
using Homemap.Domain.Common;

namespace Homemap.ApplicationCore.Models.Auth;

public class User 
{
    public Guid Id { get; set; }
    [Required]
    public string Email { get; set; } = null!;

    [Required]  
    public string PasswordHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? GoogleId { get; set; }
}
