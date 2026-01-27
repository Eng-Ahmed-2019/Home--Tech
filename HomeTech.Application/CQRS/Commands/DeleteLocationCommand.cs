using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record DeleteLocationCommand(DeleteLocationDto Dto) : IRequest<bool>;
}