using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record UpdateUserCommand(
        string UserId,
        UpdateUserDto Dto
    ) : IRequest<string>;
}