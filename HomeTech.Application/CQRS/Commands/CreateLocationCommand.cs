using MediatR;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.CQRS.Commands
{
    public record CreateLocationCommand(CreateLocationDto Dto) : IRequest<string>;
}