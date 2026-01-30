using MediatR;

namespace HomeTech.Application.CQRS.Commands
{
    public record ReactivateAccountCommand(
    string Email
) : IRequest<string>;
}