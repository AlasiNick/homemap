using Homemap.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Homemap.Domain.Core
{
    public class Project : AuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        public virtual ICollection<Receiver> Receivers { get; } = new List<Receiver>();

    }
}
