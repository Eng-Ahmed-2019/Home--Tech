using HomeTech.Domain.Enums;

namespace HomeTech.Application.DTOs
{
    public record UpdateUserDto
    {
        public string FullName { get; init; } = default!;
        public Gender Gender { get; init; }
    }
}