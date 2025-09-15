using Homemap.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Homemap.Domain.Core
{
    public class Project : AuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        public virtual ICollection<Receiver> Receivers { get; } = new List<Receiver>();

        // Owner of the project
        public Guid UserId { get; set; }
        
        // optional navigation in future if needed
        // public virtual User User { get; set; } = null!;

    }
}
