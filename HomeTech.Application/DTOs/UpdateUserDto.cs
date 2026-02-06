using HomeTech.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HomeTech.Application.DTOs
{
    public record UpdateUserDto
    {
        public string FullName { get; init; } = default!;
        public Gender Gender { get; init; }
        public IFormFile? formFile { get; set; }
    }
}