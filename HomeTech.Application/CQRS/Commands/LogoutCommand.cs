using MediatR;

namespace HomeTech.Application.CQRS.Commands
{
    public record LogoutCommand(Guid SessionId) : IRequest;
}