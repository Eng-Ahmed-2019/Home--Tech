using MediatR;

namespace HomeTech.Application.CQRS.Commands
{
    public record DeleteAccountCommand(string userId, Guid SessionId) :
        IRequest<string>;
}