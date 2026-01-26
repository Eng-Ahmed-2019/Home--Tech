using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.Interfaces
{
    public interface IJwtService
    {
        Task<LoginResponseDto> GenerateTokenAsync(ApplicationUser applicationUser, string ipAddress);
    }
}