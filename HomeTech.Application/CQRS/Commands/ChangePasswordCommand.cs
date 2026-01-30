using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record ChangePasswordCommand(
        string UserId,
        ChangePasswordDto Dto
    ) : IRequest<string>;
}