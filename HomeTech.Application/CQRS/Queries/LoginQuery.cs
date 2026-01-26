using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Queries
{
    public record LoginQuery(LoginDto Dto) : IRequest<LoginResponseDto>;
}