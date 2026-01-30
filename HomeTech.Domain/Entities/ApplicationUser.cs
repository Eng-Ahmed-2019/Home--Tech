using HomeTech.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HomeTech.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Gender Gender { set; get; }
        public string? PendingEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}