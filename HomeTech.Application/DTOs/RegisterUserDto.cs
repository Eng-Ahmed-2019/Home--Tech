using HomeTech.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HomeTech.Application.DTOs
{
    public record RegisterUserDto
    {
        public string FullName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public Gender Gender { set; get; }
        public IFormFile? formFile { set; get; }
    }
}