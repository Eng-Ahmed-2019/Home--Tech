using HomeTech.Domain.Enums;

namespace HomeTech.Application.DTOs
{
    public class UserProfileDto
    {
        public string FullName { set; get; } = default!;
        public string Email { set; get; } = default!;
        public Gender Gender { set; get; }
        public string? ImageUrl { set; get; }
        public DateTime CreatedAt { set; get; }
    }
}