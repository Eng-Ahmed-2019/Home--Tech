using MediatR;

namespace HomeTech.Application.CQRS.Commands
{
    public record ChangeEmailCommand(
    string UserId,
    string NewEmail
) : IRequest<string>;
}