using MediatR;

namespace HomeTech.Application.CQRS.Commands
{
    public record ConfirmEmailCommand(
        string Email,
        string Token
    ) : IRequest<string>;
}