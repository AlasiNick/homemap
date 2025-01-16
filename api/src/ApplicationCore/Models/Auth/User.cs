namespace Homemap.ApplicationCore.Models.Auth;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual List<UserSession>? Sessions { get; set; }
}
