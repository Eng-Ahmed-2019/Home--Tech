using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record RegisterUserCommand(
        RegisterUserDto Dto
    )
    : IRequest<string>;
}